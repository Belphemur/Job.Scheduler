using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Job.Scheduler.Utils;

namespace Job.Scheduler.Tests.Mocks
{
    public class LongRunningDebounceJob : DebounceJob
    {
        public bool HasBeenInterrupted { get; private set; }
        public LongRunningDebounceJob(List<string> list, string key, int id) : base(list, key, id)
        {
        }

        public override  async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await TaskUtils.WaitForDelayOrCancellation(TimeSpan.FromSeconds(10), cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                HasBeenInterrupted = true;
                return;
            }
            await base.ExecuteAsync(cancellationToken);
        }
    }
}