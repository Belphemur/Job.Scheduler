using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;

namespace Job.Scheduler.AspNetCore.Tests.Mock;

public class OneTimeJob : IJob
{
    public IRetryAction FailRule { get; } = new AlwaysRetry();
    public TimeSpan? MaxRuntime { get; }
    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task OnFailure(JobException exception)
    {
        return Task.CompletedTask;
    }
    
}