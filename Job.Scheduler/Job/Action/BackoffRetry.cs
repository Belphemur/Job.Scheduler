using System;

namespace Job.Scheduler.Job.Action
{
    public class BackoffRetry : IRetryAction
    {
        private readonly int? _maxRetries;
        private readonly Func<int, TimeSpan> _retryStrategy;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="retryStrategy">Implement your own strategy for delay between retries based on the current retry</param>
        /// <param name="maxRetries">null to always retry</param>
        public BackoffRetry(Func<int, TimeSpan> retryStrategy, int? maxRetries)
        {
            _maxRetries = maxRetries;
            _retryStrategy = retryStrategy;
        }

        public bool ShouldRetry(int currentRetry)
        {
            if (!_maxRetries.HasValue)
            {
                return true;
            }

            return currentRetry < _maxRetries;
        }

        public TimeSpan? GetDelayBetweenRetries(int currentRetry)
        {
            return _retryStrategy(currentRetry);
        }
    }
}