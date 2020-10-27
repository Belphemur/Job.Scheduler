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

    /// <summary>
    /// Job that is run recurringly with a delay between execution
    /// </summary>
    public interface IRecurringJob : IJob
    {
        /// <summary>
        /// Delay between job execution
        /// </summary>
        public TimeSpan Delay { get; }
    }

    /// <summary>
    /// Job that is run once after the delay has expired
    /// </summary>
    public interface IDelayedJob : IJob
    {
        /// <summary>
        /// Delay before executing the job
        /// </summary>
        public TimeSpan Delay { get; }
    }
}