using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Job.Scheduler.Builder;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Scheduler;
using Job.Scheduler.Tests.Mocks;
using Job.Scheduler.Utils;
using NSubstitute;
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
            var jobRunner = scheduler.ScheduleJobInternal(new JobScheduler.BuilderJobContainer<IJob>(job));
            await jobRunner.WaitForJob();
            job.HasRun.Should().BeTrue();
        }

        [Test]
        public async Task OneTimeJobWithOnCompleted()
        {
            IJobScheduler scheduler = new JobScheduler(_builder);
            var job = new OneTimeJob();
            
            var container = Substitute.For<IJobContainerBuilder<IJob>>();
            container.BuildJob().Returns(new JobScheduler.BuilderJobContainer<OneTimeJob>.JobContainer(job));
            container.JobType.Returns(typeof(IJob));
            
            var jobRunner = scheduler.ScheduleJobInternal(container);
            await jobRunner.WaitForJob();
            job.HasRun.Should().BeTrue();
            await container.Received(1).OnCompletedAsync(Arg.Any<CancellationToken>());
        }


        [Test]
        public async Task FailingJobShouldRetry()
        {
            IJobScheduler scheduler = new JobScheduler(_builder);
            var maxRetries = 3;
            var job = new FailingRetringJob(new RetryNTimes(maxRetries));
            var jobRunner = scheduler.ScheduleJobInternal(new JobScheduler.BuilderJobContainer<IJob>(job));
            await jobRunner.WaitForJob();
            job.Ran.Should().Be(4);
            jobRunner.Retries.Should().Be(maxRetries);
        }

        [Test]
        public async Task MaxRuntimeIsRespected()
        {
            IJobScheduler scheduler = new JobScheduler(_builder);
            var job = new MaxRuntimeJob(new NoRetry(), TimeSpan.FromMilliseconds(50));
            var jobRunner = scheduler.ScheduleJobInternal(new JobScheduler.BuilderJobContainer<IJob>(job));
            await jobRunner.WaitForJob();
            jobRunner.Elapsed.Should().BeCloseTo(job.MaxRuntime!.Value, TimeSpan.FromMilliseconds(20));
        }

        [Test]
        public async Task MaxRuntimeIsRespectedAndTaskRetried()
        {
            IJobScheduler scheduler = new JobScheduler(_builder);
            var maxRetries = 2;
            var job = new MaxRuntimeJob(new RetryNTimes(maxRetries), TimeSpan.FromMilliseconds(50));
            var jobRunner = scheduler.ScheduleJobInternal(new JobScheduler.BuilderJobContainer<IJob>(job));
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
            var jobRunner = scheduler.ScheduleJobInternal(new JobScheduler.BuilderJobContainer<IJob>(job));
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
            var jobRunner = scheduler.ScheduleJobInternal(new JobScheduler.BuilderJobContainer<IJob>(job), taskScheduler);
            await jobRunner.WaitForJob();
            job.HasRun.Should().BeTrue();
            jobRunner.Retries.Should().Be(0);
            taskScheduler.Scheduled.Should().Be(1);
            job.InitThread.Should().NotBe(job.RunThread);
            job.RunThread.Should().Be(taskScheduler.MainThread);
        }

        [Test]
        public async Task ExecuteInDefaultScheduler()
        {
            IJobScheduler scheduler = new JobScheduler(_builder);
            var job = new ThreadJob(Thread.CurrentThread);
            var jobRunner = scheduler.ScheduleJobInternal(new JobScheduler.BuilderJobContainer<IJob>(job));
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
            var jobRunnerFirst = scheduler.ScheduleJobInternal(new JobScheduler.BuilderJobContainer<IDebounceJob>(new DebounceJob(list, "Single", 0)));
            await TaskUtils.WaitForDelayOrCancellation(TimeSpan.FromMilliseconds(10), CancellationToken.None);
            var jobRunnerSecond = scheduler.ScheduleJobInternal(new JobScheduler.BuilderJobContainer<IDebounceJob>(new DebounceJob(list, "Single", 1)));
            await TaskUtils.WaitForDelayOrCancellation(TimeSpan.FromMilliseconds(150), CancellationToken.None);
            await jobRunnerFirst.WaitForJob();
            await jobRunnerSecond.WaitForJob();
            list.Should().OnlyContain(s => s == "Single1").And.HaveCount(1);
        }

        [Test]
        public async Task DebounceJobAlreadyFinishedTest()
        {
            IJobScheduler scheduler = new JobScheduler(_builder);
            var list = new List<string>();
            var jobRunnerFirst = scheduler.ScheduleJobInternal(new JobScheduler.BuilderJobContainer<IDebounceJob>(new DebounceJob(list, "Multiple", 0)));
            await TaskUtils.WaitForDelayOrCancellation(TimeSpan.FromMilliseconds(150), CancellationToken.None);
            await jobRunnerFirst.WaitForJob();
            var jobRunnerSecond = scheduler.ScheduleJobInternal(new JobScheduler.BuilderJobContainer<IDebounceJob>(new DebounceJob(list, "Multiple", 1)));
            await TaskUtils.WaitForDelayOrCancellation(TimeSpan.FromMilliseconds(150), CancellationToken.None);
            await jobRunnerSecond.WaitForJob();

            list.Should().HaveCount(2).And.ContainInOrder(new[] { "Multiple0", "Multiple1" });
        }

        
        [Test]
        public async Task LongRunningDebounceInterruptedJobTest()
        {
            IJobScheduler scheduler = new JobScheduler(_builder);
            var list = new List<string>();
            var longRunningDebounceJob = new LongRunningDebounceJob(list, "Single", 0);
            var jobRunnerFirst = scheduler.ScheduleJobInternal(new JobScheduler.BuilderJobContainer<IDebounceJob>(longRunningDebounceJob));
            await TaskUtils.WaitForDelayOrCancellation(TimeSpan.FromMilliseconds(130), CancellationToken.None);
            var jobRunnerSecond = scheduler.ScheduleJobInternal(new JobScheduler.BuilderJobContainer<IDebounceJob>(new DebounceJob(list, "Single", 1)));
            await TaskUtils.WaitForDelayOrCancellation(TimeSpan.FromMilliseconds(150), CancellationToken.None);
            await jobRunnerFirst.WaitForJob();
            await jobRunnerSecond.WaitForJob();
            longRunningDebounceJob.HasBeenInterrupted.Should().BeTrue();
            list.Should().OnlyContain(s => s == "Single1").And.HaveCount(1);
        }
        [Test]
        public void DecorrelatedBackOffTest()
        {
            var max = 10;
            var backoff = new ExponentialDecorrelatedJittedBackoffRetry(max, TimeSpan.FromSeconds(5));
            for (var i = 0; i < max; i++)
            {
                backoff.ShouldRetry(i).Should().BeTrue();
                backoff.GetDelayBetweenRetries(i);
            }

            backoff.ShouldRetry(max).Should().BeFalse();
        }
    }
}