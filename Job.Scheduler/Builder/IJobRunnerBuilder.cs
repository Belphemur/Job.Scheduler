using Job.Scheduler.Job;
using Job.Scheduler.Job.Runner;

namespace Job.Scheduler.Builder
{
    public interface IJobRunnerBuilder
    {
        /// <summary>
        /// Build a Job runner for the given job
        /// </summary>
        IJobRunner Build(IJob job);
    }
}