using System;
using System.Threading;
using System.Threading.Tasks;

namespace Job.Scheduler.Utils
{
    public static class TaskUtils
    {
        /// <summary>
        /// Return a <see cref="Task"/> that will complete (not throw) when the <paramref name="token"/> is canceled
        /// </summary>
        public static async Task WaitForTokenShutdownAsync(CancellationToken token)
        {
            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
            await using (token.Register(() => tcs.TrySetResult(null)))
            {
                await tcs.Task;
            }
        }

        /// <summary>
        /// Asynchronously waits for the <paramref name="task"/> to complete execution.
        /// </summary>
        /// <param name="task">the task to wait on</param>
        /// <param name="timeout">A <see cref="System.TimeSpan"/> that represents the number of milliseconds to wait, or a <see cref="System.TimeSpan"/> that represents -1 milliseconds to wait indefinitely. </param>
        /// <returns>true if the <paramref name="task"/> completed execution within the allotted time; otherwise, false.</returns>
        public static async Task<bool> WaitAsync(this Task task, TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource();
            var completedTask = await Task.WhenAny(Task.Delay(timeout, cts.Token), task);
            if (completedTask == task)
            {
                // Cancel the timer task so that it does not fire
                cts.Cancel();
                return true;
            }
            return false;
        }
    }
}