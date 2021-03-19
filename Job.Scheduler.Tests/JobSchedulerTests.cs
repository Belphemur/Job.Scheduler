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
            var jobId = _scheduler.ScheduleJob(job);
            var jobRunner = _scheduler.GetJobRunner(jobId);
            await jobRunner.WaitForJob();
            job.HasRun.Should().BeTrue();
            _scheduler.HasJob(jobId).Should().BeFalse();
        }


        [Test]
        public async Task FailingJobShouldRetry()
        {
            var job = new FailingRetringJob();
            var jobId = _scheduler.ScheduleJob(job);
            var jobRunner = _scheduler.GetJobRunner(jobId);
            await jobRunner.WaitForJob();
            job.Ran.Should().Be(4);
            jobRunner.Retries.Should().Be(3);
        }

        [Test]
        public async Task MaxRuntimeIsRespected()
        {
            var job = new MaxRuntimeJob(new NoRetry());
            var jobId = _scheduler.ScheduleJob(job);
            var jobRunner = _scheduler.GetJobRunner(jobId);
            await jobRunner.WaitForJob();
            jobRunner.Elapsed.Should().BeCloseTo(job.MaxRuntime!.Value);
        }

        [Test]
        public async Task MaxRuntimeIsRespectedAndTaskRetried()
        {
            var job = new MaxRuntimeJob(new RetryNTimes(2));
            var jobId = _scheduler.ScheduleJob(job);
            var jobRunner = _scheduler.GetJobRunner(jobId);
            await jobRunner.WaitForJob();
            jobRunner.Elapsed.Should().BeCloseTo(job.MaxRuntime!.Value * 3);
        }
    }
}