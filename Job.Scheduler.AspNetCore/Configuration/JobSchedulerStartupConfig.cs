using Job.Scheduler.AspNetCore.Builder;
using Job.Scheduler.Job;
using Job.Scheduler.Queue;

namespace Job.Scheduler.AspNetCore.Configuration;

/// <summary>
/// Help setting up the job we want to have running at startup
/// </summary>
public class JobSchedulerStartupConfig
{
    private readonly IJobBuilder _jobBuilder;

    public JobSchedulerStartupConfig(IJobBuilder jobBuilder)
    {
        _jobBuilder = jobBuilder;
    }

    internal readonly List<IContainerJob<IQueueJob>> QueueJobs = new();
    internal readonly List<IContainerJob<IJob>> OneTimeJobs = new();

    internal readonly List<IContainerJob<IDebounceJob>> DebounceJobs = new();

    internal readonly List<IContainerJob<IDelayedJob>> DelayedJobs = new();

    internal readonly List<IContainerJob<IRecurringJob>> RecurringJobs = new();

    private readonly List<QueueSettings> _queueSettings = new();

    /// <summary>
    /// Add job that will be run at startup
    /// </summary>
    /// <param name="jobBuilder"></param>
    /// <returns></returns>
    public JobSchedulerStartupConfig AddStartupJob(Func<IJobBuilder, IContainerJob<IQueueJob>> jobBuilder)
    {
        QueueJobs.Add(jobBuilder(_jobBuilder));
        return this;
    }

    /// <summary>
    /// Add job that will be run at startup
    /// </summary>
    /// <param name="jobBuilder"></param>
    /// <returns></returns>
    public JobSchedulerStartupConfig AddStartupJob(Func<IJobBuilder, IContainerJob<IJob>> jobBuilder)
    {
        OneTimeJobs.Add(jobBuilder(_jobBuilder));
        return this;
    }

    /// <summary>
    /// Add job that will be run at startup
    /// </summary>
    /// <param name="jobBuilder"></param>
    /// <returns></returns>
    public JobSchedulerStartupConfig AddStartupJob(Func<IJobBuilder, IContainerJob<IDebounceJob>> jobBuilder)
    {
        DebounceJobs.Add(jobBuilder(_jobBuilder));
        return this;
    }

    /// <summary>
    /// Add job that will be run at startup
    /// </summary>
    /// <param name="jobBuilder"></param>
    /// <returns></returns>
    public JobSchedulerStartupConfig AddStartupJob(Func<IJobBuilder, IContainerJob<IDelayedJob>> jobBuilder)
    {
        DelayedJobs.Add(jobBuilder(_jobBuilder));
        return this;
    }

    /// <summary>
    /// Add job that will be run at startup
    /// </summary>
    /// <param name="jobBuilder"></param>
    /// <returns></returns>
    public JobSchedulerStartupConfig AddStartupJob(Func<IJobBuilder, IContainerJob<IRecurringJob>> jobBuilder)
    {
        RecurringJobs.Add(jobBuilder(_jobBuilder));
        return this;
    }

    /// <summary>
    /// Register specific queue
    /// </summary>
    /// <param name="queueSettings"></param>
    /// <returns></returns>
    public JobSchedulerStartupConfig RegisterQueue(QueueSettings queueSettings)
    {
        _queueSettings.Add(queueSettings);
        return this;
    }

    internal IEnumerable<QueueSettings> QueueSettings => _queueSettings;
}