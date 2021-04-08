using System;
using System.Threading.Tasks;
using FluentAssertions;
using Job.Scheduler.Builder;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Scheduler;
using Job.Scheduler.Tests.Mocks;
using NUnit.Framework;

namespace Job.Scheduler.Tests
{
    [Parallelizable(ParallelScope.Children)]
    public class Tests
    {
        private IJobScheduler _scheduler;
        private IJobRunnerBuilder _builder;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _builder = new JobRunnerBuilder();
        }

        [SetUp]
        public void Setup()
        {
            _scheduler = new JobScheduler(_builder);
        }

        [Test]
        public async Task OneTimeJob()
        {
            var job = new OneTimeJob();
            var jobRunner = _scheduler.ScheduleJobInternal(job);
            await jobRunner.WaitForJob();
            job.HasRun.Should().BeTrue();
        }


        [Test]
        public async Task FailingJobShouldRetry()
        {
            var maxRetries = 3;
            var job = new FailingRetringJob(new RetryNTimes(maxRetries));
            var jobRunner = _scheduler.ScheduleJobInternal(job);
            await jobRunner.WaitForJob();
            job.Ran.Should().Be(4);
            jobRunner.Retries.Should().Be(maxRetries);
        }

        [Test]
        public async Task MaxRuntimeIsRespected()
        {
            var job = new MaxRuntimeJob(new NoRetry());
            var jobRunner = _scheduler.ScheduleJobInternal(job);
            await jobRunner.WaitForJob();
            jobRunner.Elapsed.Should().BeCloseTo(job.MaxRuntime!.Value, TimeSpan.FromMilliseconds(20));
        }

        [Test]
        public async Task MaxRuntimeIsRespectedAndTaskRetried()
        {
            var maxRetries = 2;
            var job = new MaxRuntimeJob(new RetryNTimes(maxRetries));
            var jobRunner = _scheduler.ScheduleJobInternal(job);
            await jobRunner.WaitForJob();
            jobRunner.Elapsed.Should().BeCloseTo(job.MaxRuntime!.Value, TimeSpan.FromMilliseconds(20));
            jobRunner.Retries.Should().Be(maxRetries);
        }
    }
}