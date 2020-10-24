using System.Threading;
using System.Threading.Tasks;

namespace Job.Scheduler.Job.Runner
{
    internal class DelayedJobRunner : JobRunner<IDelayedJob>
    {
        public DelayedJobRunner(IDelayedJob job) : base(job)
        {
        }

        protected override async Task StartJobAsync(IDelayedJob job, CancellationToken token)
        {
            await Task.Delay(job.Delay, token);
            if (token.IsCancellationRequested)
            {
                return;
            }

            await ExecuteJob(job, token);
        }
    }
}