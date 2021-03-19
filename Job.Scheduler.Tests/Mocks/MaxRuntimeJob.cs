using System;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;

namespace Job.Scheduler.Tests.Mocks
{
    public class MaxRuntimeJob : IJob
    {
        public IRetryAction FailRule { get; } = new NoRetry();
        public TimeSpan? MaxRuntime { get; } = TimeSpan.FromSeconds(1);

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                await Task.Delay(10, cancellationToken);
            }
        }

        public Task OnFailure(JobException exception)
        {
            return Task.CompletedTask;
        }
    }
}