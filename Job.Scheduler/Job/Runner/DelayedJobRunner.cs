using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Job.Scheduler.Utils;

namespace Job.Scheduler.Job.Runner
{
    internal class DelayedJobRunner : JobRunner<IDelayedJob>
    {
        public DelayedJobRunner(IJobContainerBuilder<IDelayedJob> builderJobContainer, Func<IJobRunner, Task> jobDone, [CanBeNull] TaskScheduler taskScheduler) : base(builderJobContainer, jobDone, taskScheduler)
        {
        }

        protected override async Task StartJobAsync(IJobContainerBuilder<IDelayedJob> builderJobContainer, CancellationToken token)
        {
            using var jobContainer = builderJobContainer.BuildJob();
            var job = jobContainer.Job;
            await TaskUtils.WaitForDelayOrCancellation(job.Delay, token);
            if (token.IsCancellationRequested)
            {
                return;
            }

            await InnerExecuteJob(job, token);
        }
    }
}