using System;

namespace Job.Scheduler.Job.Action
{
    /// <summary>
    /// Should always retry the job
    /// </summary>
    public class AlwaysRetry : IRetryAction
    {
        public TimeSpan? DelayBetweenRetries { get; }

        public bool ShouldRetry(int currentRetry)
        {
            return true;
        }
    }
}