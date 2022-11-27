using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Job.Scheduler.Utils;

namespace Job.Scheduler.Job.Runner
{
    internal class RecurringJobRunner : JobRunner<IRecurringJob>
    {
        public RecurringJobRunner(IJobContainerBuilder<IRecurringJob> builderJobContainer, Func<IJobRunner, Task> jobDone, [CanBeNull] TaskScheduler taskScheduler) : base(builderJobContainer, jobDone, taskScheduler)
        {
        }

        protected override async Task StartJobAsync(IJobContainerBuilder<IRecurringJob> builderJobContainer, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                using var jobContainer = builderJobContainer.BuildJob();
                var job = jobContainer.Job;
                await InnerExecuteJob(job, token);

                await TaskUtils.WaitForDelayOrCancellation(job.Delay, token);
            }
        }
    }
}