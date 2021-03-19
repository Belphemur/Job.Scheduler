using System.Threading.Tasks;
using FluentAssertions;
using Job.Scheduler.Builder;
using Job.Scheduler.Scheduler;
using Job.Scheduler.Tests.Mocks;
using NUnit.Framework;

namespace Job.Scheduler.Tests
{
    public class Tests
    {
        private IJobScheduler _scheduler;

        [SetUp]
        public void Setup()
        {
            _scheduler = new JobScheduler(new JobRunnerBuilder());
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
        public async Task RecurringJobShouldRetry()
        {
            var job = new RecurringJobRetry();
            var jobId = _scheduler.ScheduleJob(job);
            var jobRunner = _scheduler.GetJobRunner(jobId);
            await jobRunner.WaitForJob();
            job.Ran.Should().Be(4);
        }

        [Test]
        public async Task MaxRuntimeIsRespected()
        {
            var job = new MaxRuntimeJob();
            var jobId = _scheduler.ScheduleJob(job);
            var jobRunner = _scheduler.GetJobRunner(jobId);
            await jobRunner.WaitForJob();
            jobRunner.Elapsed.Should().BeCloseTo(job.MaxRuntime!.Value);
        }
    }
}