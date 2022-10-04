using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;

namespace Job.Scheduler.AspNetCore.Tests.Mock;

public class HasRunJob : IJob
{
    public class Runstate
    {
        public bool HasRun;
    }

    public Runstate Run = null!;

    public IRetryAction FailRule { get; } = new NoRetry();
    public TimeSpan? MaxRuntime { get; }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Run.HasRun = true;
        return Task.CompletedTask;
    }

    public Task OnFailure(JobException exception)
    {
        return Task.CompletedTask;
    }
}