using System;
using System.Threading.Tasks;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Runner;

namespace Job.Scheduler.Builder
{
    public interface IJobRunnerBuilder
    {
        /// <summary>
        /// Build a Job runner for the given job
        /// </summary>
        IJobRunner Build<TJob>(IContainerJob<TJob> job, Func<IJobRunner, Task> jobDone, TaskScheduler taskScheduler) where TJob : IJob;
    }
}