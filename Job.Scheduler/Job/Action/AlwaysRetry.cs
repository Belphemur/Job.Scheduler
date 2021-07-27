using System;

namespace Job.Scheduler.Job.Action
{
    /// <summary>
    /// Should always retry the job
    /// </summary>
    public class AlwaysRetry : IRetryAction
    {
        private readonly TimeSpan? _delayBetweenRetries;

        public AlwaysRetry(TimeSpan? delayBetweenRetries = null)
        {
            _delayBetweenRetries = delayBetweenRetries;
        }

        public bool ShouldRetry(int currentRetry)
        {
            return true;
        }

        public TimeSpan? GetDelayBetweenRetries(int currentRetry)
        {
            return _delayBetweenRetries;
        }
    }
}