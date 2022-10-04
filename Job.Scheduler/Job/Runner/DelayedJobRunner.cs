using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Job.Scheduler.Utils;

namespace Job.Scheduler.Job.Runner
{
    internal class DelayedJobRunner : JobRunner<IDelayedJob>
    {
        public DelayedJobRunner(IContainerJob<IDelayedJob> jobContainer, Func<IJobRunner, Task> jobDone, [CanBeNull] TaskScheduler taskScheduler) : base(jobContainer, jobDone, taskScheduler)
        {
        }

        protected override async Task StartJobAsync(IContainerJob<IDelayedJob> jobContainer, CancellationToken token)
        {
            var job = jobContainer.BuildJob();
            await TaskUtils.WaitForDelayOrCancellation(job.Delay, token);
            if (token.IsCancellationRequested)
            {
                return;
            }

            await InnerExecuteJob(job, token);
        }
    }
}