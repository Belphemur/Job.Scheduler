using System;

namespace Job.Scheduler.Job.Action
{
    public interface IRetryAction
    {
        /// <summary>
        /// Should the job be retried
        /// </summary>
        /// <param name="currentRetry"></param>
        bool ShouldRetry(int currentRetry);

        /// <summary>
        /// Should there be a delay between the retries.
        ///
        /// Also you're able to define you're own backoff strategy using the <see cref="currentRetry"/>.
        /// 
        /// </summary>
        /// <param name="currentRetry"></param>
        /// <returns></returns>
        public TimeSpan? GetDelayBetweenRetries(int currentRetry);
    }
}