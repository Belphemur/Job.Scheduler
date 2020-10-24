using System.Threading;
using System.Threading.Tasks;

namespace Job.Scheduler.Job.Runner
{
    internal class RecurringJobRunner : JobRunner<IRecurringJob>
    {
        public RecurringJobRunner(IRecurringJob job) : base(job)
        {
        }

        protected override async Task StartJobAsync(IRecurringJob job, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (!await ExecuteJob(job, token)) break;

                await Task.Delay(job.Delay, token);
            }
        }
    }
}