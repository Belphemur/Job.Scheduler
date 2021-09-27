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
        /// <param name="job"></param>
        /// <param name="token"></param>
        /// <param name="taskScheduler"></param>
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
        /// <param name="job"></param>
        /// <param name="taskScheduler"></param>
        /// <param name="token"></param>
        internal IJobRunner ScheduleJobInternal(IJob job, TaskScheduler taskScheduler = null, CancellationToken token = default);
    }
}