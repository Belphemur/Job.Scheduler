using System;
using System.Threading;
using System.Threading.Tasks;

namespace Job.Scheduler.Job.Runner
{
    internal class OneTimeJobRunner : JobRunner<IJob>
    {
        protected override Task StartJobAsync(IJob job, CancellationToken token)
        {
            return InnerExecuteJob(job, token);
        }

        public OneTimeJobRunner(IJob job, Func<IJobRunner, Task> jobDone) : base(job, jobDone)
        {
        }
    }
}