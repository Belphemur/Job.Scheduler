using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Job.Scheduler.Job.Runner
{
    internal class OneTimeJobRunner : JobRunner<IJob>
    {
        public OneTimeJobRunner(IContainerJob<IJob> jobContainer, Func<IJobRunner, Task> jobDone, [CanBeNull] TaskScheduler taskScheduler) : base(jobContainer, jobDone, taskScheduler)
        {
        }

        protected override Task StartJobAsync(IContainerJob<IJob> jobContainer, CancellationToken token)
        {
            var job = jobContainer.BuildJob();
            return InnerExecuteJob(job, token);
        }
        
    }
}