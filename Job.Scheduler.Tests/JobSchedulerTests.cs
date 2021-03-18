using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Job.Scheduler.Builder;
using Job.Scheduler.Job;
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

            public Task ExecuteAsync(CancellationToken cancellationToken)
            {
                HasRun = true;
                return Task.CompletedTask;
            }

            public Task<bool> OnFailure(JobException exception)
            {
                return Task.FromResult(true);
            }
        }

        public class RecurringJobRetry : IRecurringJob
        {
            public bool HasRun { get; private set; }

            public int Retry { get; private set; } = 0;

            public Task ExecuteAsync(CancellationToken cancellationToken)
            {
                Retry++;
                if (Retry < 3)
                {
                    throw new Exception("Test");
                }

                HasRun = true;
                return Task.CompletedTask;
            }

            public Task<bool> OnFailure(JobException exception)
            {
                return Task.FromResult(Retry < 3);
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
            _scheduler.ScheduleJob(job);
            await _scheduler.StopAsync();
            job.HasRun.Should().BeTrue();
        }


        [Test]
        public async Task RecurringJobShouldRetry()
        {
            var job = new RecurringJobRetry();
            _scheduler.ScheduleJob(job);
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            await _scheduler.StopAsync();
            job.HasRun.Should().BeTrue();
            job.Retry.Should().Be(3);
        }
    }
}