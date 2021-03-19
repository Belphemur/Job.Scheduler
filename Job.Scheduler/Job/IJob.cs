using System;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;

namespace Job.Scheduler.Job
{
    /// <summary>
    /// Interface to implement to define your job
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// What to do if the job fails
        /// <see cref="AlwaysRetry"/>, <see cref="NoRetry"/> or <see cref="RetryNTimes"/>.
        ///
        /// If null, it's considered to be <see cref="NoRetry"/>
        /// </summary>
        public IRetryAction FailRule { get; }

        /// <summary>
        /// Define the max runtime of a job before it's considered to have failed.
        ///
        /// Set to NULL if no maximum
        /// </summary>
        public TimeSpan? MaxRuntime { get; }

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
        /// <returns>What action to take now, doesn't have to be the one that was taken before.</returns>
        Task OnFailure(JobException exception);
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