using System;

namespace Job.Scheduler.Job.Action
{
    /// <summary>
    /// Retry the job
    /// </summary>
    public class RetryNTimes : IRetryAction
    {
        private readonly int _maxRetries;
        private readonly TimeSpan? _delayBetweenRetries;


        public RetryNTimes(int maxRetries, TimeSpan? delayBetweenRetries = null)
        {
            _maxRetries = maxRetries;
            _delayBetweenRetries = delayBetweenRetries;
        }

        public bool ShouldRetry(int currentRetry)
        {
            return currentRetry < _maxRetries;
        }

        public TimeSpan? GetDelayBetweenRetries(int currentRetry)
        {
            return _delayBetweenRetries;
        }
    }
}