using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Job.Scheduler.Utils;

namespace Job.Scheduler.Job.Runner
{
    internal class DebounceJobRunner : JobRunner<IDebounceJob>
    {
        public DebounceJobRunner(IJobContainerBuilder<IDebounceJob> builderJobContainer, Func<IJobRunner, bool, Task> jobDone, [CanBeNull] TaskScheduler taskScheduler) : base(builderJobContainer, jobDone, taskScheduler)
        {
        }

        public override string Key => BuilderJobContainer.Key;

        protected override async Task StartJobAsync(IJobContainerBuilder<IDebounceJob> builderJobContainer, CancellationToken token)
        {
            using var jobContainer = builderJobContainer.BuildJob();
            var job = jobContainer.Job;
            await InnerExecuteJob(job, token);
        }
    }
}