﻿using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Job.Scheduler.Job.Runner
{
    internal class OneTimeJobRunner : JobRunner<IJob>
    {
        public OneTimeJobRunner(IJobContainerBuilder<IJob> builderJobContainer,  Func<IJobRunner, bool, Task> jobDone, [CanBeNull] TaskScheduler taskScheduler) : base(builderJobContainer, jobDone, taskScheduler)
        {
        }

        protected override async Task StartJobAsync(IJobContainerBuilder<IJob> builderJobContainer, CancellationToken token)
        {
            using var jobContainer = builderJobContainer.BuildJob();
            var job = jobContainer.Job;
            await InnerExecuteJob(job, token);
        }
        
    }
}