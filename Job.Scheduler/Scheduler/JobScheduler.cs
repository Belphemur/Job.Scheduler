using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job;

namespace Job.Scheduler.Scheduler
{
    public class JobScheduler : IJobScheduler
    {
        private readonly ConcurrentDictionary<Guid, JobRunner> _jobs = new ConcurrentDictionary<Guid, JobRunner>();

        /// <summary>
        /// Schedule a new job to run
        /// </summary>
        /// <param name="job"></param>
        /// <param name="token"></param>
        public void ScheduleJob(IJob job, CancellationToken token = default)
        {
            var runner = new JobRunner(job);
            runner.JobDone += (sender, args) => _jobs.Remove(((JobRunner) sender).UniqueId, out _);
            _jobs.TryAdd(runner.UniqueId, runner);
            runner.Start(token);
        }

        /// <summary>
        /// Stop asynchronously any running job
        /// </summary>
        /// <returns></returns>
        public Task StopAsync()
        {
            return Task.WhenAll(_jobs.Values.Select(runner => runner.StopAsync()));
        }
    }
}