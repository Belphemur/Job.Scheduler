﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Job.Scheduler.Job.Runner
{
    /// <summary>
    /// Used to run the job
    /// </summary>
    public interface IJobRunner
    {
        /// <summary>
        /// Unique ID of the job runner
        /// </summary>
        Guid UniqueId { get; }

        /// <summary>
        /// Type of the job that is run by the runner
        /// </summary>
        Type JobType { get; }

        /// <summary>
        /// Is the job still running
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// For how long is the job running
        /// </summary>
        TimeSpan Elapsed { get; }

        /// <summary>
        /// Number of time the job has been retried
        /// </summary>
        int Retries { get; }

        /// <summary>
        /// Key of the job, used for deduplication
        /// </summary>
        string Key { get; }

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
        public Task<TimeSpan> StopAsync(CancellationToken token);

        /// <summary>
        /// Wait for the task to end
        /// </summary>
        /// <returns></returns>
        internal Task WaitForJob();
    }
}