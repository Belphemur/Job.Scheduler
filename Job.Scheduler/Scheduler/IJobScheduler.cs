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
        JobId ScheduleJob(IJob job, CancellationToken token = default);

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
        /// Used for test to be able to get the <see cref="IJobRunner"/>
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        internal IJobRunner GetJobRunner(JobId jobId);
    }
}