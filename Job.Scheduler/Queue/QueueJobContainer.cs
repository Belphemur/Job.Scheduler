using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job;

namespace Job.Scheduler.Queue;

internal record QueueJobContainer(IJobContainerBuilder<IQueueJob> JobContainer, TaskScheduler TaskScheduler, CancellationToken Token)
{
    public string Key => JobContainer.Key;
}