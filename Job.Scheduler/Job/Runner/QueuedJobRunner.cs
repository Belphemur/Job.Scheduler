using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Job.Scheduler.Job.Runner;

internal class QueuedJobRunner : JobRunner<IQueueJob>
{
    public QueuedJobRunner(IJobContainerBuilder<IQueueJob> builderJobContainer, Func<IJobRunner, Task> jobDone, [CanBeNull] TaskScheduler taskScheduler) : base(builderJobContainer, jobDone, taskScheduler)
    {
    }

    protected override async Task StartJobAsync(IJobContainerBuilder<IQueueJob> builderJobContainer, CancellationToken token)
    {
        using var jobContainer = builderJobContainer.BuildJob();
        var job = jobContainer.Job;
        await InnerExecuteJob(job, token);
    }
}