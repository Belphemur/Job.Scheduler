using System;
using System.Threading;
using System.Threading.Tasks;

namespace Job.Scheduler.Job.Runner
{
    public interface IJobRunner
    {
        event EventHandler JobDone;

        /// <summary>
        /// Unique ID of the job runner
        /// </summary>
        Guid UniqueId { get; }

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
        Task StopAsync();
    }
}