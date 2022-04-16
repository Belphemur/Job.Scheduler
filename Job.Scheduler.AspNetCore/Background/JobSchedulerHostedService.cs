#region

using Job.Scheduler.Job;
using Job.Scheduler.Scheduler;
using Microsoft.Extensions.Hosting;

#endregion

namespace Job.Scheduler.AspNetCore.Background;

public class JobSchedulerHostedService : IHostedService
{
    private readonly IJobScheduler _jobScheduler;
    private readonly IEnumerable<IContainerJob> _startupJobs;

    public JobSchedulerHostedService(IJobScheduler jobScheduler, IEnumerable<IContainerJob> startupJobs)
    {
        _jobScheduler = jobScheduler;
        _startupJobs = startupJobs;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var containerJob in _startupJobs)
        {
            _jobScheduler.ScheduleJob(containerJob, cancellationToken);
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _jobScheduler.StopAsync(cancellationToken);
    }
}