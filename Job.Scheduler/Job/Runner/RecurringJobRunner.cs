using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Job.Scheduler.Utils;

namespace Job.Scheduler.Job.Runner
{
    internal class RecurringJobRunner : JobRunner<IRecurringJob>
    {
        public RecurringJobRunner(IContainerJob<IRecurringJob> jobContainer, Func<IJobRunner, Task> jobDone, [CanBeNull] TaskScheduler taskScheduler) : base(jobContainer, jobDone, taskScheduler)
        {
        }

        protected override async Task StartJobAsync(IContainerJob<IRecurringJob> jobContainer, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var job = jobContainer.BuildJob();
                await InnerExecuteJob(job, token);

                await TaskUtils.WaitForDelayOrCancellation(job.Delay, token);
            }
        }
    }
}