using System;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Job.Scheduler.Utils;

namespace Job.Scheduler.Tests.Mocks
{
    public class OneTimeQueueJob : IQueueJob
    {
        private readonly TimeSpan _waitTime;

        public OneTimeQueueJob(TimeSpan waitTime)
        {
            _waitTime = waitTime;
        }

        public bool HasRun { get; set; }

        public IRetryAction FailRule { get; } = new NoRetry();
        public TimeSpan? MaxRuntime { get; }

        public virtual async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await TaskUtils.WaitForDelayOrCancellation(_waitTime, cancellationToken);
            HasRun = true;
        }

        public Task OnFailure(JobException exception)
        {
            return Task.FromResult<IRetryAction>(new AlwaysRetry());
        }

        public string Key { get; set; } = "test";
        public string QueueId { get; set; } = "test";
    }
}