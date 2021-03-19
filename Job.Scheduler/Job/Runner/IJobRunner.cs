using System;
using System.Threading;
using System.Threading.Tasks;

namespace Job.Scheduler.Job.Runner
{
    /// <summary>
    /// Used to run the job
    /// </summary>
    public interface IJobRunner : IDisposable
    {
        /// <summary>
        /// Unique ID of the job runner
        /// </summary>
        Guid UniqueId { get; }

        /// <summary>
        /// Is the job still running
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Run the job
        /// </summary>
        /// <param name="token">Optional token to sync with it</param>
        /// <returns></returns>
        void Start(CancellationToken token = default);

        /// <summary>
        /// Stop the task and wait for it to terminate
        /// </summary>
        /// <returns></returns>
        public Task StopAsync(CancellationToken token);
        
        /// <summary>
        /// Wait for the task to end
        /// </summary>
        /// <returns></returns>
        internal Task WaitForJob();
    }
}