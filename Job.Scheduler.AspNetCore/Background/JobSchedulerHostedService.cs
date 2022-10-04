#region

using Job.Scheduler.AspNetCore.Configuration;
using Job.Scheduler.Job;
using Job.Scheduler.Scheduler;
using Microsoft.Extensions.Hosting;

#endregion

namespace Job.Scheduler.AspNetCore.Background;

public class JobSchedulerHostedService : IHostedService
{
    private readonly IJobScheduler _jobScheduler;
    private readonly JobSchedulerStartupConfig _config;

    public JobSchedulerHostedService(IJobScheduler jobScheduler, JobSchedulerStartupConfig config)
    {
        _jobScheduler = jobScheduler;
        _config = config;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var queueSetting in _config.QueueSettings)
        {
            _jobScheduler.RegisterQueue(queueSetting);
        }

        foreach (var containerJob in _config.QueueJobs)
        {
            _jobScheduler.ScheduleJob(containerJob, cancellationToken);
        }

        foreach (var containerJob in _config.DebounceJobs)
        {
            _jobScheduler.ScheduleJob(containerJob, cancellationToken);
        }

        foreach (var containerJob in _config.DelayedJobs)
        {
            _jobScheduler.ScheduleJob(containerJob, cancellationToken);
        }

        foreach (var containerJob in _config.RecurringJobs)
        {
            _jobScheduler.ScheduleJob(containerJob, cancellationToken);
        }

        foreach (var containerJob in _config.OneTimeJobs)
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