using System;

namespace Job.Scheduler.Job.Action
{
    /// <summary>
    /// Don't retry the job
    /// </summary>
    public class NoRetry : IRetryAction
    {

        public bool ShouldRetry(int currentRetry)
        {
            return false;
        }

        public TimeSpan? GetDelayBetweenRetries(int currentRetry)
        {
            return null;
        }
    }
}