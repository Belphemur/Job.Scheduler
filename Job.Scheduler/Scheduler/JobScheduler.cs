using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Builder;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Data;
using Job.Scheduler.Job.Runner;

namespace Job.Scheduler.Scheduler
{
    /// <summary>
    /// Takes care of scheduling new <see cref="IJob"/> and managing them.
    /// </summary>
    public class JobScheduler : IJobScheduler
    {
        private readonly ConcurrentDictionary<Guid, IJobRunner> _jobs = new ConcurrentDictionary<Guid, IJobRunner>();
        private readonly IJobRunnerBuilder _jobRunnerBuilder;

        public JobScheduler(IJobRunnerBuilder jobRunnerBuilder)
        {
            _jobRunnerBuilder = jobRunnerBuilder;
        }

        /// <summary>
        /// Schedule a new job to run
        /// </summary>
        /// <param name="job">The job to run</param>
        /// <param name="token">If you want to cancel easily this specific job later. Default = None</param>
        /// <param name="taskScheduler">In which TaskScheduler should the job be run. Default = TaskScheduler.Default</param>
        public JobId ScheduleJob(IJob job, CancellationToken token = default, TaskScheduler taskScheduler = null)
        {
            var runner = ((IJobScheduler) this).ScheduleJobInternal(job, taskScheduler, token);
            return new JobId(runner.UniqueId);
        }

        /// <summary>
        /// Stop the given job
        /// </summary>
        public async Task StopAsync(JobId jobId, CancellationToken token)
        {
            _jobs.TryGetValue(jobId.UniqueId, out var jobRunner);
            if (jobRunner == null)
            {
                return;
            }

            await jobRunner.StopAsync(token);
        }

        /// <summary>
        /// Is the job present in the scheduler
        /// </summary>
        public bool HasJob(JobId jobId) => _jobs.TryGetValue(jobId.UniqueId, out _);

        IJobRunner IJobScheduler.ScheduleJobInternal(IJob job, TaskScheduler taskScheduler, CancellationToken token)
        {
            var runner = _jobRunnerBuilder.Build(job, jobRunner =>
            {
                _jobs.Remove(jobRunner.UniqueId, out _);
                return Task.CompletedTask;
            }, taskScheduler);
            _jobs.TryAdd(runner.UniqueId, runner);
            runner.Start(token);
            return runner;
        }

        /// <summary>
        /// Stop the task and wait for it to terminate.
        /// Use the token to stop the task earlier
        /// </summary>
        public Task StopAsync(CancellationToken token = default)
        {
            return Task.WhenAll(_jobs.Values.Select(runner => runner.StopAsync(token)));
        }
    }
}