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

    private readonly List<IContainerJob> _jobs = new();
    private readonly List<QueueSettings> _queueSettings = new();

    /// <summary>
    /// Add job that will be run at startup
    /// </summary>
    /// <param name="jobBuilder"></param>
    /// <returns></returns>
    public JobSchedulerStartupConfig AddStartupJob(Func<IJobBuilder, IContainerJob> jobBuilder)
    {
        _jobs.Add(jobBuilder(_jobBuilder));
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

    internal IEnumerable<IContainerJob> Jobs => _jobs;
    internal IEnumerable<QueueSettings> QueueSettings => _queueSettings;
}