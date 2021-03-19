using System;

namespace Job.Scheduler.Job.Action
{
    /// <summary>
    /// Don't retry the job
    /// </summary>
    public class NoRetry : IRetryAction
    {
        public TimeSpan? DelayBetweenRetries { get; }

        public bool ShouldRetry(int currentRetry)
        {
            return false;
        }
    }
}