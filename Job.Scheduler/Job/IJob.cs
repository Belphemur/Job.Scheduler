using System;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job.Exception;

namespace Job.Scheduler.Job
{
    public interface IJob
    {
        /// <summary>
        /// Execute the job
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ExecuteAsync(CancellationToken cancellationToken);

        /// <summary>
        /// What to do on failure
        /// </summary>
        /// <param name="exception"></param>
        /// <returns>True to retry, false to stop</returns>
        Task<bool> OnFailure(JobException exception);
    }

    public interface IRecurringJob : IJob
    {
        /// <summary>
        /// Delay between job execution
        /// </summary>
        public TimeSpan Delay { get; }
    }
}