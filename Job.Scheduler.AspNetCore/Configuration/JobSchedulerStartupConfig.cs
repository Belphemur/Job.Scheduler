using Job.Scheduler.AspNetCore.Builder;
using Job.Scheduler.Job;

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

    internal IEnumerable<IContainerJob> Jobs => _jobs;
}