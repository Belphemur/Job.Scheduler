using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job;

namespace Job.Scheduler.Queue;

internal record QueueJobContainer(IContainerJob Container, TaskScheduler TaskScheduler, CancellationToken Token)
{
    public string Key => ((IQueueJob)Container.Job).Key;
}