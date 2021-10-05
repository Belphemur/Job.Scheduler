using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Job.Scheduler.Builder;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Scheduler;
using Job.Scheduler.Tests.Mocks;
using Job.Scheduler.Utils;
using NUnit.Framework;

namespace Job.Scheduler.Tests
{
    [Parallelizable(ParallelScope.Children)]
    public class Tests
    {
        private IJobRunnerBuilder _builder;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _builder = new JobRunnerBuilder();
        }

        [Test]
        public async Task OneTimeJob()
        {
            IJobScheduler scheduler = new JobScheduler(_builder);
            var job = new OneTimeJob();
            var jobRunner = scheduler.ScheduleJobInternal(job);
            await jobRunner.WaitForJob();
            job.HasRun.Should().BeTrue();
        }


        [Test]
        public async Task FailingJobShouldRetry()
        {
            IJobScheduler scheduler = new JobScheduler(_builder);
            var maxRetries = 3;
            var job = new FailingRetringJob(new RetryNTimes(maxRetries));
            var jobRunner = scheduler.ScheduleJobInternal(job);
            await jobRunner.WaitForJob();
            job.Ran.Should().Be(4);
            jobRunner.Retries.Should().Be(maxRetries);
        }

        [Test]
        public async Task MaxRuntimeIsRespected()
        {
            IJobScheduler scheduler = new JobScheduler(_builder);
            var job = new MaxRuntimeJob(new NoRetry(), TimeSpan.FromMilliseconds(50));
            var jobRunner = scheduler.ScheduleJobInternal(job);
            await jobRunner.WaitForJob();
            jobRunner.Elapsed.Should().BeCloseTo(job.MaxRuntime!.Value, TimeSpan.FromMilliseconds(20));
        }

        [Test]
        public async Task MaxRuntimeIsRespectedAndTaskRetried()
        {
            IJobScheduler scheduler = new JobScheduler(_builder);
            var maxRetries = 2;
            var job = new MaxRuntimeJob(new RetryNTimes(maxRetries), TimeSpan.FromMilliseconds(50));
            var jobRunner = scheduler.ScheduleJobInternal(job);
            await jobRunner.WaitForJob();
            jobRunner.Elapsed.Should().BeCloseTo(job.MaxRuntime!.Value, TimeSpan.FromMilliseconds(20));
            jobRunner.Retries.Should().Be(maxRetries);
        }


        [Test]
        public async Task MaxRuntimeIsRespectedAndTaskRetriedWithBackoff()
        {
            IJobScheduler scheduler = new JobScheduler(_builder);
            var maxRetries = 3;
            var job = new MaxRuntimeJob(new ExponentialBackoffRetry(TimeSpan.FromMilliseconds(10), maxRetries), TimeSpan.FromMilliseconds(80));
            var jobRunner = scheduler.ScheduleJobInternal(job);
            await jobRunner.WaitForJob();
            jobRunner.Elapsed.Should().BeCloseTo(job.MaxRuntime!.Value, TimeSpan.FromMilliseconds(20));
            jobRunner.Retries.Should().Be(maxRetries);
        }

        [Test]
        public async Task ExecuteInOwnScheduler()
        {
            IJobScheduler scheduler = new JobScheduler(_builder);
            using var taskScheduler = new MockTaskScheduler();
            var job = new ThreadJob(Thread.CurrentThread);
            var jobRunner = scheduler.ScheduleJobInternal(job, taskScheduler);
            await jobRunner.WaitForJob();
            job.HasRun.Should().BeTrue();
            jobRunner.Retries.Should().Be(0);
            taskScheduler.Count.Should().Be(1);
            job.InitThread.Should().NotBe(job.RunThread);
            job.RunThread.Should().Be(taskScheduler.MainThread);
        }

        [Test]
        public async Task ExecuteInDefaultScheduler()
        {
            IJobScheduler scheduler = new JobScheduler(_builder);
            var job = new ThreadJob(Thread.CurrentThread);
            var jobRunner = scheduler.ScheduleJobInternal(job);
            await jobRunner.WaitForJob();
            job.HasRun.Should().BeTrue();
            jobRunner.Retries.Should().Be(0);
            job.InitThread.Should().Be(job.RunThread);
        }

        [Test]
        public async Task DebounceJobTest()
        {
            IJobScheduler scheduler = new JobScheduler(_builder);
            var list = new List<string>();
            var job = new DebounceJob(list, "Single");
            var jobRunnerFirst = scheduler.ScheduleJobInternal(job);
            await TaskUtils.WaitForDelayOrCancellation(TimeSpan.FromMilliseconds(10), CancellationToken.None);
            var jobRunnerSecond = scheduler.ScheduleJobInternal(job);
            await jobRunnerFirst.WaitForJob();
            await jobRunnerSecond.WaitForJob();

            list.Should().ContainSingle(job.Key);
        }

        [Test]
        public async Task DebounceJobAlreadyFinishedTest()
        {
            IJobScheduler scheduler = new JobScheduler(_builder);
            var list = new List<string>();
            var job = new DebounceJob(list, "Multiple");
            var jobRunnerFirst = scheduler.ScheduleJobInternal(job);
            await jobRunnerFirst.WaitForJob();
            var jobRunnerSecond = scheduler.ScheduleJobInternal(job);
            await jobRunnerSecond.WaitForJob();

            list.Should().OnlyContain(s => s == job.Key).And.HaveCount(2);
        }
    }
}