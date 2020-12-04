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
        /// <see href="https://stackoverflow.com/questions/30739788/cancel-task-delay-without-exception-or-use-exception-to-control-flow"/>
        /// </summary>
        public static async Task WaitForDelayOrCancellation(TimeSpan delay, CancellationToken token)
        {
            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            var cts = new CancellationTokenSource();
            await using var _ = token
                .Register(o =>
                    {
                        tcs.SetResult(null);
                        cts.Cancel();
                    },
                    null);

            await Task.WhenAny(tcs.Task, Task.Delay(delay, cts.Token));
        }
    }
}