using System.Threading;
using System.Threading.Tasks;

namespace Job.Scheduler.Job.Runner
{
    internal class DelayedJobRunner : JobRunner<IDelayedJob>
    {
        public DelayedJobRunner(IDelayedJob job) : base(job)
        {
        }

        protected override Task StartJobAsync(IDelayedJob job, CancellationToken token)
        {
            return RunAsyncWithDone(async cancellationToken =>
            {
                await Task.Delay(job.Delay, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await ExecuteJob(job, cancellationToken);
            }, token);
        }
    }
}