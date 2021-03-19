using System;

namespace Job.Scheduler.Job.Action
{
    public interface IRetryAction
    {
        /// <summary>
        /// What's the delay between the retries if needed
        /// </summary>
        TimeSpan? DelayBetweenRetries { get; }

        /// <summary>
        /// Should the job be retried
        /// </summary>
        /// <param name="currentRetry"></param>
        bool ShouldRetry(int currentRetry);
    }
}