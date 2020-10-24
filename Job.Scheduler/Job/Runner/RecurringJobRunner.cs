using System.Threading;
using System.Threading.Tasks;

namespace Job.Scheduler.Job.Runner
{
    internal class RecurringJobRunner : JobRunner<IRecurringJob>
    {
        public RecurringJobRunner(IRecurringJob job) : base(job)
        {
        }

        protected override Task StartJobAsync(IRecurringJob job, CancellationToken token)
        {
            return RunAsyncWithDone(async cancellationToken =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (!await ExecuteJob(job, cancellationToken)) break;

                    await Task.Delay(job.Delay, cancellationToken);
                }
            }, token);
        }
    }
}