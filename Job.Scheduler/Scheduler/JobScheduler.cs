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
using Job.Scheduler.Queue;

namespace Job.Scheduler.Scheduler
{
    /// <summary>
    /// Takes care of scheduling new <see cref="IJob"/> and managing them.
    /// </summary>
    public class JobScheduler : IJobScheduler
    {
        private readonly ConcurrentDictionary<Guid, IJobRunner> _jobs = new();
        private readonly ConcurrentDictionary<string, Debouncer> _debouncedJobs = new();
        private readonly IJobRunnerBuilder _jobRunnerBuilder;
        private readonly ConcurrentDictionary<string, Queue.Queue> _queues = new();

        internal class BuilderJobContainer<TJob> : IJobContainerBuilder<TJob> where TJob : IJob
        {
            internal class JobContainer : IJobContainer<TJob>
            {
                public JobContainer(TJob job)
                {
                    Job = job;
                }

                public void Dispose()
                {
                }

                public TJob Job { get; }
            }
            private readonly TJob _job;
            

            public Type JobType => typeof(TJob);
            public string Key { get; }
            public string QueueId { get; }

            public Task OnCompletedAsync(CancellationToken token)
            {
                return Task.CompletedTask;
            }

            public IJobContainer<TJob> BuildJob()
            {
                return new JobContainer(_job);
            }

            public BuilderJobContainer(TJob job)
            {
                _job = job;
                Key = _job is HasKey keyed ? keyed.Key : Guid.NewGuid().ToString();
                QueueId = _job is IQueueJob queueJob ? queueJob.QueueId : null;
            }
        }

        public JobScheduler(IJobRunnerBuilder jobRunnerBuilder)
        {
            _jobRunnerBuilder = jobRunnerBuilder;
        }


        /// <summary>
        /// Register a queue
        /// </summary>
        /// <param name="queueSettings"></param>
        /// <exception cref="ArgumentException">Queue already exists</exception>
        public void RegisterQueue(QueueSettings queueSettings)
        {
            if (!_queues.TryAdd(queueSettings.QueueId, new Queue.Queue(queueSettings, _jobRunnerBuilder)))
            {
                throw new ArgumentException($"Already have a queue registered with that Id: {queueSettings.QueueId}", nameof(queueSettings));
            }
        }

        Queue.Queue IJobScheduler.GetQueue(string queueId)
        {
            _queues.TryGetValue(queueId, out var queue);
            return queue;
        }

        /// <summary>
        /// Schedule a new job to run through a container setup
        /// </summary>
        /// <param name="builderJobContainer">The container of the job to run</param>
        /// <param name="token">If you want to cancel easily this specific job later. Default = None</param>
        /// <param name="taskScheduler">In which TaskScheduler should the job be run. Default = TaskScheduler.Default</param>
        public JobId ScheduleJob<TJob>(IJobContainerBuilder<TJob> builderJobContainer, CancellationToken token = default, TaskScheduler taskScheduler = null) where TJob : IJob
        {
            var runner = ((IJobScheduler)this).ScheduleJobInternal(builderJobContainer, taskScheduler, token);
            return runner == null ? new JobId() : new JobId(runner.UniqueId);
        }

        /// <summary>
        /// Schedule a new job to run
        /// </summary>
        /// <param name="job">The job to run</param>
        /// <param name="token">If you want to cancel easily this specific job later. Default = None</param>
        /// <param name="taskScheduler">In which TaskScheduler should the job be run. Default = TaskScheduler.Default</param>
        public JobId ScheduleJob<TJob>(TJob job, CancellationToken token = default, TaskScheduler taskScheduler = null) where TJob : IJob => ScheduleJob(new BuilderJobContainer<TJob>(job), token, taskScheduler);

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

        IJobRunner IJobScheduler.ScheduleJobInternal<TJob>(IJobContainerBuilder<TJob> builderJobContainer, TaskScheduler taskScheduler, CancellationToken token)
        {
            if (builderJobContainer is IJobContainerBuilder<IQueueJob> queueJobContainer)
            {
                using var jobContainer = queueJobContainer.BuildJob();
                var job = jobContainer.Job;
                return HandleQueueJobs(queueJobContainer, taskScheduler, job.QueueId, token);
            }

            if (builderJobContainer is IJobContainerBuilder<IDebounceJob> debounceContainer)
            {
                return HandleDebounceJobs(taskScheduler, debounceContainer, token);
            }
            

            var runner = _jobRunnerBuilder.Build(builderJobContainer, async jobRunner =>
            {
                _jobs.TryRemove(jobRunner.UniqueId, out _);
                await builderJobContainer.OnCompletedAsync(token);
            }, taskScheduler);
            _jobs.TryAdd(runner.UniqueId, runner);
            

            runner.Start(token);
            return runner;
        }

        private IJobRunner HandleDebounceJobs(TaskScheduler taskScheduler, IJobContainerBuilder<IDebounceJob> debounceContainer, CancellationToken token)
        {
            using var jobContainer = debounceContainer.BuildJob();
            var debounceJob = jobContainer.Job;
            if (_debouncedJobs.TryGetValue(debounceJob.Key, out var debouncer))
            {
                debouncer.Debounce(debounceContainer);
                return debouncer.JobRunner;
            }

            var debounceRunner = (_jobRunnerBuilder.Build(debounceContainer, async jobRunner =>
            {
                _jobs.TryRemove(jobRunner.UniqueId, out _);
                _debouncedJobs.TryRemove(debounceJob.Key, out var finishedDebouncer);
                finishedDebouncer?.Dispose();
                await debounceContainer.OnCompletedAsync(token);
            }, taskScheduler) as DebounceJobRunner)!;
            
            debouncer = new Debouncer(debounceJob, debounceRunner);
            _jobs.TryAdd(debounceRunner.UniqueId, debounceRunner);
            _debouncedJobs.TryAdd(debounceJob.Key, debouncer);

            debouncer.Start();
            return debouncer.JobRunner;
        }

        private IJobRunner HandleQueueJobs(IJobContainerBuilder<IQueueJob> builderJobContainer, TaskScheduler taskScheduler, string queueId, CancellationToken cancellationToken)
        {
            if (!_queues.TryGetValue(queueId, out var queue))
            {
                throw new ArgumentException($"Can't schedule job on a non registered queue: {queueId}. Use {nameof(RegisterQueue)} with your {nameof(QueueSettings)}.");
            }

            queue.AddJob(new QueueJobContainer(builderJobContainer, taskScheduler, cancellationToken));
            return null;
        }

        /// <summary>
        /// Stop the task and wait for it to terminate.
        /// Use the token to stop the task earlier
        /// </summary>
        public Task StopAsync(CancellationToken token = default)
        {
            return Task.WhenAll(_jobs.Values.Select(runner => runner.StopAsync(token)).Concat(_queues.Values.Select(queue => queue.StopAsync(token))));
        }
    }
}