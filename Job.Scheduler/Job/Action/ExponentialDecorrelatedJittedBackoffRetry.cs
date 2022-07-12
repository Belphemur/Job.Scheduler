using System;
using System.Collections.Generic;
using Polly.Contrib.WaitAndRetry;

namespace Job.Scheduler.Job.Action;

/// <summary>
/// Add some jitter to the exponential backoff to avoid having the job retrying at the same time
/// </summary>
public class ExponentialDecorrelatedJittedBackoffRetry : IRetryAction
{
    private readonly Lazy<Queue<TimeSpan>> _retryTimes;

    /// <summary>
    /// Add some jitter to the exponential backoff to avoid having the job retrying at the same time
    /// </summary>
    /// <param name="maxRetries"></param>
    /// <param name="medianDelay"></param>
    public ExponentialDecorrelatedJittedBackoffRetry(int maxRetries, TimeSpan medianDelay)
    {
        _retryTimes = new Lazy<Queue<TimeSpan>>(() => new Queue<TimeSpan>(Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: medianDelay, retryCount: maxRetries)));
    }
    
    public bool ShouldRetry(int currentRetry)
    {
        return _retryTimes.Value.Count > 0;
    }

    public TimeSpan? GetDelayBetweenRetries(int currentRetry)
    {
        return _retryTimes.Value.Dequeue();
    }
}