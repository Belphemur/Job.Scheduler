using System;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;

namespace Job.Scheduler.Tests.Mocks
{
    public class OneTimeJob : IJob
    {
        public bool HasRun { get; private set; }

        public IRetryAction FailRule { get; } = new NoRetry();
        public TimeSpan? MaxRuntime { get; }

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            HasRun = true;
            return Task.CompletedTask;
        }

        public Task OnFailure(JobException exception)
        {
            return Task.FromResult<IRetryAction>(new AlwaysRetry());
        }
    }
}