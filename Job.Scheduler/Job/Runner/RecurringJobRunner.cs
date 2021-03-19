using System;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Utils;

namespace Job.Scheduler.Job.Runner
{
    internal class RecurringJobRunner : JobRunner<IRecurringJob>
    {
        public RecurringJobRunner(IRecurringJob job, Func<IJobRunner, Task> jobDone) : base(job, jobDone)
        {
        }

        protected override async Task StartJobAsync(IRecurringJob job, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await ExecuteJob(job, token);

                await TaskUtils.WaitForDelayOrCancellation(job.Delay, token);
            }
        }
    }
}