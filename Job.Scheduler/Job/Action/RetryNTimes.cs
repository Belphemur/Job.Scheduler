using System;

namespace Job.Scheduler.Job.Action
{
    /// <summary>
    /// Retry the job
    /// </summary>
    public class RetryNTimes : IRetryAction
    {
        private readonly int _maxRetries;

        public TimeSpan? DelayBetweenRetries { get; }

        public RetryNTimes(int maxRetries, TimeSpan? delayBetweenRetries = null)
        {
            _maxRetries = maxRetries;
            DelayBetweenRetries = delayBetweenRetries;
        }

        public bool ShouldRetry(int currentRetry)
        {
            return currentRetry < _maxRetries;
        }
    }
}