using System;
using System.Threading;
using System.Threading.Tasks;

namespace Job.Scheduler.Utils
{
    public static class TaskUtils
    {
        /// <summary>
        /// Wait for the given delay or a cancellation token to expire.
        ///
        /// Doesn't trigger an exception
        /// </summary>
        public static async Task WaitForDelayOrCancellation(TimeSpan delay, CancellationToken token)
        {
            try
            {
                await Task.Delay(delay, token);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}