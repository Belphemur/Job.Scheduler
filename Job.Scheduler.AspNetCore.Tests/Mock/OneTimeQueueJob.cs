using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Job.Scheduler.Utils;

namespace Job.Scheduler.AspNetCore.Tests.Mock
{
    public class OneTimeQueueJob : IQueueJob
    {
        public class RunStateInfo
        {
            public bool HasRun { get; set; }
        }
        public TimeSpan WaitTime { get; set; }


        public RunStateInfo RunState { get; set; }

        public IRetryAction FailRule { get; } = new NoRetry();
        public TimeSpan? MaxRuntime { get; }

        public virtual async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await TaskUtils.WaitForDelayOrCancellation(WaitTime, cancellationToken);
            RunState.HasRun = true;
        }

        public Task OnFailure(JobException exception)
        {
            return Task.CompletedTask;
        }

        public string Key { get; set; } = "test";
        public string QueueId { get; set; } = "test";
    }
}