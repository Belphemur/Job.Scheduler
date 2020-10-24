using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job;

namespace Job.Scheduler.Scheduler
{
    public interface IJobScheduler
    {
        /// <summary>
        /// Schedule a new job to run
        /// </summary>
        /// <param name="job"></param>
        /// <param name="token"></param>
        void ScheduleJob(IJob job, CancellationToken token = default);

        /// <summary>
        /// Stop asynchronously any running job
        /// Use the token to stop the job earlier if needed
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task StopAsync(CancellationToken token = default);
    }
}