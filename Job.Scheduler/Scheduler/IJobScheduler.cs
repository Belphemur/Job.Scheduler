using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Data;
using Job.Scheduler.Job.Runner;

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
        JobId ScheduleJob(IJob job, CancellationToken token = default, TaskScheduler taskScheduler = null);

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
        /// <param name="jobContainer"></param>
        /// <param name="taskScheduler"></param>
        /// <param name="token"></param>
        IJobRunner ScheduleJobInternal(IContainerJob jobContainer, TaskScheduler taskScheduler = null, CancellationToken token = default);

        /// <summary>
        /// Schedule a new job to run through a container setup
        /// </summary>
        /// <param name="jobContainer">The container of the job to run</param>
        /// <param name="token">If you want to cancel easily this specific job later. Default = None</param>
        /// <param name="taskScheduler">In which TaskScheduler should the job be run. Default = TaskScheduler.Default</param>
        JobId ScheduleJob(IContainerJob jobContainer, CancellationToken token = default, TaskScheduler taskScheduler = null);

    }
}