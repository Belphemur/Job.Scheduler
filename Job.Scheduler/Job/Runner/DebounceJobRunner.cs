using System;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Utils;

namespace Job.Scheduler.Job.Runner
{
    internal class DebounceJobRunner : JobRunner<IDebounceJob>
    {
        public override string Key => _job.Key;

        protected override async Task StartJobAsync(IDebounceJob job, CancellationToken token)
        {
            await TaskUtils.WaitForDelayOrCancellation(job.DebounceTime, token);
            if (token.IsCancellationRequested)
            {
                return;
            }

            InnerExecuteJob(job, token);
        }

        public DebounceJobRunner(IDebounceJob job, Func<IJobRunner, Task> jobDone, TaskScheduler taskScheduler) : base(job, jobDone, taskScheduler)
        {
        }
    }
}