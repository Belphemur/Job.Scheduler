using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Job.Scheduler.Builder;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Job.Scheduler.Scheduler;
using NUnit.Framework;

namespace Job.Scheduler.Tests
{
    public class Tests
    {
        public class OnTimeJob : IJob
        {
            public bool HasRun { get; private set; }

            public IRetryAction FailRule { get; } = new NoRetry();

            public Task ExecuteAsync(CancellationToken cancellationToken)
            {
                HasRun = true;
                return Task.CompletedTask;
            }

            public Task OnFailure(JobException exception)
            {
                return Task.FromResult<IRetryAction>(new AlwaysRetry());
            }
        }

        public class RecurringJobRetry : IRecurringJob
        {

            public int Ran { get; private set; }

            public IRetryAction FailRule { get; } = new RetryNTimes(3);

            public Task ExecuteAsync(CancellationToken cancellationToken)
            {
                Ran++;
                throw new Exception("Test");
            }

            public Task OnFailure(JobException exception)
            {
                return Task.CompletedTask;
            }

            public TimeSpan Delay { get; } = TimeSpan.FromMilliseconds(10);
        }

        private IJobScheduler _scheduler;

        [SetUp]
        public void Setup()
        {
            _scheduler = new JobScheduler(new JobRunnerBuilder());
        }

        [Test]
        public async Task OneTimeJob()
        {
            var job = new OnTimeJob();
            var jobId = _scheduler.ScheduleJob(job);
            await _scheduler.StopAsync();
            job.HasRun.Should().BeTrue();
            _scheduler.HasJob(jobId).Should().BeFalse();
        }


        [Test]
        public async Task RecurringJobShouldRetry()
        {
            var job = new RecurringJobRetry();
            _scheduler.ScheduleJob(job);
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            await _scheduler.StopAsync();
            job.Ran.Should().Be(4);
        }
    }
}