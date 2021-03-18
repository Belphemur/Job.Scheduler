using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Builder;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Data;
using Job.Scheduler.Job.Runner;

namespace Job.Scheduler.Scheduler
{
    public class JobScheduler : IJobScheduler
    {
        private readonly ConcurrentDictionary<Guid, IJobRunner> _jobs = new ConcurrentDictionary<Guid, IJobRunner>();
        private readonly IJobRunnerBuilder _jobRunnerBuilder;

        public JobScheduler(IJobRunnerBuilder jobRunnerBuilder)
        {
            _jobRunnerBuilder = jobRunnerBuilder;
        }

        /// <summary>
        /// Schedule a new job to run
        /// </summary>
        /// <param name="job"></param>
        /// <param name="token"></param>
        public JobId ScheduleJob(IJob job, CancellationToken token = default)
        {
            var runner = _jobRunnerBuilder.Build(job, async jobRunner =>
            {
                _jobs.Remove(jobRunner.UniqueId, out _);
                try
                {
                    await jobRunner.StopAsync(default);
                }
                finally
                {
                    jobRunner.Dispose();
                }
            });
            _jobs.TryAdd(runner.UniqueId, runner);
            runner.Start(token);
            return new JobId(runner.UniqueId);
        }

        /// <summary>
        /// Stop the given job
        /// </summary>
        public async Task StopAsync(JobId jobId, CancellationToken token)
        {
            _jobs.TryGetValue(jobId.UniqueId, out var jobRunner);
            if (jobRunner == null)
            {
                return;
            }

            await jobRunner.StopAsync(token);
        }

        /// <summary>
        /// Is the job present in the scheduler
        /// </summary>
        public bool HasJob(JobId jobId) => _jobs.TryGetValue(jobId.UniqueId, out _);

        /// <summary>
        /// Stop the task and wait for it to terminate.
        /// Use the token to stop the task earlier
        /// </summary>
        public Task StopAsync(CancellationToken token = default)
        {
            return Task.WhenAll(_jobs.Values.Select(runner => runner.StopAsync(token)));
        }
    }
}