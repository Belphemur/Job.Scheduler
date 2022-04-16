using System;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;

namespace Job.Scheduler.Tests.Mocks
{

    public class FailingRetringJob : IJob
    {

        public int Ran { get; private set; }

        public IRetryAction FailRule { get; }
        public TimeSpan? MaxRuntime { get; }

        public FailingRetringJob(IRetryAction failRule)
        {
            FailRule = failRule;
        }

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Ran++;
            throw new Exception("Test");
        }

        public Task OnFailure(JobException exception)
        {
            return Task.CompletedTask;
        }

        public TimeSpan Delay { get; } = TimeSpan.FromMilliseconds(10);
    }
}