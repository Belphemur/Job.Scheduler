using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

#nullable enable
namespace Job.Scheduler.Job;

/// <summary>
/// Container of a job used to wrap a job and handle case of disposing
/// </summary>
public interface IContainerJob<out TJob>
{
    /// <summary>
    /// Ran when the job has finished running
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task OnCompletedAsync(CancellationToken token);

    /// <summary>
    /// Build the job to be run
    /// </summary>
    /// <returns></returns>
    public TJob BuildJob();
    
    /// <summary>
    /// Type of the contained job
    /// </summary>
    public Type JobType { get; }
    
    /// <summary>
    /// Key of the job
    /// </summary>
    public string Key { get; }
    
    /// <summary>
    /// Id of queue if present
    /// </summary>
    public string? QueueId { get; }
}