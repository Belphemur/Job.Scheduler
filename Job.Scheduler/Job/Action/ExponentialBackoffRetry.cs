using System;

namespace Job.Scheduler.Job.Action
{
    /// <summary>
    /// Exponential backoff strategy where we wait more and more between retries
    /// </summary>
    public class ExponentialBackoffRetry : BackoffRetry
    {
        /// <summary>
        /// Exponential backoff
        /// </summary>
        /// <param name="baseDelay">Base delay that will be exponentially increased at each retries</param>
        /// <param name="maxRetries">null = always retries. Any other value simple set the maximum of retries</param>
        public ExponentialBackoffRetry(TimeSpan baseDelay, int? maxRetries) : base(currentRetry => TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * Math.Pow(2, currentRetry)), maxRetries)
        {
        }
    }
}