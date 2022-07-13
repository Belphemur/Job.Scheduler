using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Job.Scheduler.Job.Runner;

internal class QueuedJobRunner : JobRunner<IQueueJob>
{
    public QueuedJobRunner(IQueueJob job, Func<IJobRunner, Task> jobDone, [CanBeNull] TaskScheduler taskScheduler) : base(job, jobDone, taskScheduler)
    {
    }

    protected override Task StartJobAsync(IQueueJob job, CancellationToken token)
    {
        return InnerExecuteJob(job, token);
    }
}