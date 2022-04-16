using System;
using System.Threading;
using System.Threading.Tasks;

namespace Job.Scheduler.Job;

/// <summary>
/// Container of a job used to wrap a job and handle case of disposing
/// </summary>
public interface IContainerJob
{
    /// <summary>
    /// Job that is contained
    /// </summary>
    IJob Job { get; }

    /// <summary>
    /// Ran when the job has finished running
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task OnCompletedAsync(CancellationToken token);
}