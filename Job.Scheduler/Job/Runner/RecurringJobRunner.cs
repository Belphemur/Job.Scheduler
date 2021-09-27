using System;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Utils;

namespace Job.Scheduler.Job.Runner
{
    internal class RecurringJobRunner : JobRunner<IRecurringJob>
    {
        public RecurringJobRunner(IRecurringJob job, Func<IJobRunner, Task> jobDone, TaskScheduler taskScheduler) : base(job, jobDone, taskScheduler)
        {
        }

        protected override async Task StartJobAsync(IRecurringJob job, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await InnerExecuteJob(job, token);

                await TaskUtils.WaitForDelayOrCancellation(job.Delay, token);
            }
        }
    }
}