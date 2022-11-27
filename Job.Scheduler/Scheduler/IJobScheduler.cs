using System;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Data;
using Job.Scheduler.Job.Runner;
using Job.Scheduler.Queue;

namespace Job.Scheduler.Scheduler
{
    public interface IJobScheduler
    {
        /// <summary>
        /// Schedule a new job to run
        /// </summary>
        /// <param name="job">The job to run</param>
        /// <param name="token">If you want to cancel easily this specific job later. Default = None</param>
        /// <param name="taskScheduler">In which TaskScheduler should the job be run. Default = TaskScheduler.Default</param>
        public JobId ScheduleJob<TJob>(TJob job, CancellationToken token = default, TaskScheduler taskScheduler = null) where TJob : IJob;

        /// <summary>
        /// Stop asynchronously any running job
        /// Use the token to stop the job earlier if needed
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task StopAsync(CancellationToken token = default);

        /// <summary>
        /// Stop the given job
        /// </summary>
        Task StopAsync(JobId jobId, CancellationToken token);

        /// <summary>
        /// Is the job present in the scheduler
        /// </summary>
        bool HasJob(JobId jobId);

        /// <summary>
        /// Schedule a new job to run, internal
        /// </summary>
        /// <param name="builderJobContainer"></param>
        /// <param name="taskScheduler"></param>
        /// <param name="token"></param>
        internal IJobRunner ScheduleJobInternal<TJob>(IJobContainerBuilder<TJob> builderJobContainer, TaskScheduler taskScheduler = null, CancellationToken token = default) where TJob : IJob;

        /// <summary>
        /// Schedule a new job to run through a container setup
        /// </summary>
        /// <param name="builderJobContainer">The container of the job to run</param>
        /// <param name="token">If you want to cancel easily this specific job later. Default = None</param>
        /// <param name="taskScheduler">In which TaskScheduler should the job be run. Default = TaskScheduler.Default</param>
        JobId ScheduleJob<TJob>(IJobContainerBuilder<TJob> builderJobContainer, CancellationToken token = default, TaskScheduler taskScheduler = null) where TJob : IJob;

        /// <summary>
        /// Register a queue
        /// </summary>
        /// <param name="queueSettings"></param>
        /// <exception cref="ArgumentException">Queue already exists</exception>
        void RegisterQueue(QueueSettings queueSettings);

        /// <summary>
        /// Get the queue from the id
        /// </summary>
        /// <param name="queueId"></param>
        /// <returns></returns>
        internal Queue.Queue GetQueue(string queueId);
    }
}