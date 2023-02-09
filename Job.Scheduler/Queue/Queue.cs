using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Builder;
using Job.Scheduler.Job.Runner;

namespace Job.Scheduler.Queue;

internal class Queue
{
    private readonly QueueSettings Settings;
    private readonly IJobRunnerBuilder _jobRunnerBuilder;
    private readonly ConcurrentQueue<QueueJobContainer> _jobs = new();
    private readonly ConcurrentDictionary<string, QueueJobContainer> _jobsPerKey = new();
    private readonly ConcurrentDictionary<Guid, IJobRunner> _runningJobs = new();
    private bool _stopping;


    public Queue(QueueSettings settings, IJobRunnerBuilder jobRunnerBuilder)
    {
        Settings = settings;
        _jobRunnerBuilder = jobRunnerBuilder;
    }

    internal IEnumerable<IJobRunner> RunningJobs => _runningJobs.Values;

    internal IEnumerable<QueueJobContainer> QueuedJobs => _jobsPerKey.Values;

    internal int QueuedJobsCount => _jobsPerKey.Count;

    /// <summary>
    /// Add a job to the queue
    /// </summary>
    /// <param name="queueJobContainer"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public bool AddJob(QueueJobContainer queueJobContainer)
    {
        if (_stopping)
        {
            return false;
        }

        var container = queueJobContainer.JobContainer;
        if (container.QueueId != Settings.QueueId)
        {
            throw new ArgumentException($"Can't schedule a job with wrong queueID. Expected {Settings.QueueId} got {container.QueueId}", nameof(queueJobContainer));
        }

        if (_jobsPerKey.ContainsKey(container.Key))
        {
            return false;
        }

        _jobsPerKey.TryAdd(container.Key, queueJobContainer);
        _jobs.Enqueue(queueJobContainer);

        ScheduleJobsToConcurrency();

        return true;
    }

    private void ScheduleJobsToConcurrency()
    {
        if (_stopping)
        {
            return;
        }

        if (_runningJobs.Count >= Settings.MaxConcurrency)
        {
            return;
        }

        while (_runningJobs.Count < Settings.MaxConcurrency && _jobs.TryDequeue(out var job))
        {
            ScheduleJob(job);
            _jobsPerKey.TryRemove(job.Key, out _);
        }
    }

    private void ScheduleJob(QueueJobContainer containerJob)
    {
        var jobRunner = _jobRunnerBuilder.Build(containerJob.JobContainer, async (runner, stoppedManually) =>
        {
            try
            {
                await containerJob.JobContainer.OnCompletedAsync(containerJob.Token);
            }
            finally
            {
                _runningJobs.TryRemove(runner.UniqueId, out _);
                ScheduleJobsToConcurrency();
            }
        }, containerJob.TaskScheduler);
        _runningJobs.TryAdd(jobRunner.UniqueId, jobRunner);
        jobRunner.Start(containerJob.Token);
    }

    /// <summary>
    /// Stop the queue and any of its running jobs
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task StopAsync(CancellationToken token)
    {
        _stopping = true;
        _jobs.Clear();
        _jobsPerKey.Clear();
        return Task.WhenAll(_runningJobs.Values.Select(runner => runner.StopAsync(token)));
    }
}