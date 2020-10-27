using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Utils;

namespace Job.Scheduler.Job.Runner
{
    internal class DelayedJobRunner : JobRunner<IDelayedJob>
    {
        public DelayedJobRunner(IDelayedJob job) : base(job)
        {
        }

        protected override async Task StartJobAsync(IDelayedJob job, CancellationToken token)
        {
            await TaskUtils.WaitForDelayOrCancellation(job.Delay, token);
            if (token.IsCancellationRequested)
            {
                return;
            }

            await ExecuteJob(job, token);
        }
    }
}