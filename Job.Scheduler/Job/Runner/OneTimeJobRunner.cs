using System.Threading;
using System.Threading.Tasks;

namespace Job.Scheduler.Job.Runner
{
    internal class OneTimeJobRunner : JobRunner<IJob>
    {
        public OneTimeJobRunner(IJob job) : base(job)
        {
        }

        protected override Task StartJobAsync(IJob job, CancellationToken token)
        {
            return ExecuteJob(job, token);
        }
    }
}