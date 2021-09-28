using System;
using System.Threading;
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
            var job = new MaxRuntimeJob(new NoRetry(), TimeSpan.FromMilliseconds(50));
            var jobRunner = _scheduler.ScheduleJobInternal(job);
            await jobRunner.WaitForJob();
            jobRunner.Elapsed.Should().BeCloseTo(job.MaxRuntime!.Value, TimeSpan.FromMilliseconds(20));
        }

        [Test]
        public async Task MaxRuntimeIsRespectedAndTaskRetried()
        {
            var maxRetries = 2;
            var job = new MaxRuntimeJob(new RetryNTimes(maxRetries), TimeSpan.FromMilliseconds(50));
            var jobRunner = _scheduler.ScheduleJobInternal(job);
            await jobRunner.WaitForJob();
            jobRunner.Elapsed.Should().BeCloseTo(job.MaxRuntime!.Value, TimeSpan.FromMilliseconds(20));
            jobRunner.Retries.Should().Be(maxRetries);
        }


        [Test]
        public async Task MaxRuntimeIsRespectedAndTaskRetriedWithBackoff()
        {
            var maxRetries = 3;
            var job = new MaxRuntimeJob(new ExponentialBackoffRetry(TimeSpan.FromMilliseconds(10), maxRetries), TimeSpan.FromMilliseconds(80));
            var jobRunner = _scheduler.ScheduleJobInternal(job);
            await jobRunner.WaitForJob();
            jobRunner.Elapsed.Should().BeCloseTo(job.MaxRuntime!.Value, TimeSpan.FromMilliseconds(20));
            jobRunner.Retries.Should().Be(maxRetries);
        }

        [Test]
        public async Task ExecuteInOwnScheduler()
        {
            using var scheduler = new MockTaskScheduler();
            var job = new ThreadJob(Thread.CurrentThread);
            var jobRunner = _scheduler.ScheduleJobInternal(job, scheduler);
            await jobRunner.WaitForJob();
            job.HasRun.Should().BeTrue();
            jobRunner.Retries.Should().Be(0);
            scheduler.Count.Should().Be(1);
            job.InitThread.Should().NotBe(job.RunThread);
            job.RunThread.Should().Be(scheduler.MainThread);
        }
        
        [Test]
        public async Task ExecuteInDefaultScheduler()
        {
            var job = new ThreadJob(Thread.CurrentThread);
            var jobRunner = _scheduler.ScheduleJobInternal(job);
            await jobRunner.WaitForJob();
            job.HasRun.Should().BeTrue();
            jobRunner.Retries.Should().Be(0);
            job.InitThread.Should().Be(job.RunThread);
        }
    }
}