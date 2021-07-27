using System;

namespace Job.Scheduler.Job.Action
{
    public class ExponentialBackoffRetry : BackoffRetry
    {
        public ExponentialBackoffRetry(TimeSpan baseDelay, int? maxRetries) : base(currentRetry => TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * Math.Pow(2, currentRetry)), maxRetries)
        {
        }
    }
}