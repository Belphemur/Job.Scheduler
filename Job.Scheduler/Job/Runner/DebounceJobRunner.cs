using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Job.Scheduler.Utils;

namespace Job.Scheduler.Job.Runner
{
    internal class DebounceJobRunner : JobRunner<IDebounceJob>
    {
        public DebounceJobRunner(IContainerJob<IDebounceJob> jobContainer, Func<IJobRunner, Task> jobDone, [CanBeNull] TaskScheduler taskScheduler) : base(jobContainer, jobDone, taskScheduler)
        {
        }

        public override string Key => _jobContainer.Key;

        protected override async Task StartJobAsync(IContainerJob<IDebounceJob> jobContainer, CancellationToken token)
        {
            var job = jobContainer.BuildJob();
            await TaskUtils.WaitForDelayOrCancellation(job.DebounceTime, token);
            if (token.IsCancellationRequested)
            {
                return;
            }

            await InnerExecuteJob(job, token);
        }
    }
}