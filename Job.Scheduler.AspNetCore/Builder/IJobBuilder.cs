using Job.Scheduler.Job;

namespace Job.Scheduler.AspNetCore.Builder;

public interface IJobBuilder
{
    /// <summary>
    /// Create a job container of the given type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    JobBuilder.Container<T> Create<T>() where T : IJob;
}