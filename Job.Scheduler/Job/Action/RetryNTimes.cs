using System;

namespace Job.Scheduler.Job.Action
{
    /// <summary>
    /// Retry the job
    /// </summary>
    public class RetryNTimes : IRetryAction
    {
        /// <summary>
        /// Number of times to retry the job
        /// </summary>
        public int MaxRetries { get; }

        public TimeSpan? DelayBetweenRetries { get; }

        public RetryNTimes(int maxRetries, TimeSpan? delayBetweenRetries = null)
        {
            MaxRetries = maxRetries;
            DelayBetweenRetries = delayBetweenRetries;
        }

        public bool ShouldRetry(int currentRetry)
        {
            return currentRetry < MaxRetries;
        }
    }
}