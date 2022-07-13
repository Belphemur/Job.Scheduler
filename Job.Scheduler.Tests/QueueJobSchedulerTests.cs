using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Job.Scheduler.Builder;
using Job.Scheduler.Queue;
using Job.Scheduler.Scheduler;
using Job.Scheduler.Tests.Mocks;
using NUnit.Framework;

namespace Job.Scheduler.Tests;

[Parallelizable(ParallelScope.Children)]
public class QueueJobSchedulerTests
{
    private IJobRunnerBuilder _builder;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _builder = new JobRunnerBuilder();
    }

    [Test]
    public void ThrowExceptionRegisterTwiceQueue()
    {
        var scheduler = new JobScheduler(_builder);
        var settings = new QueueSettings("test", 1);
        scheduler.RegisterQueue(settings);
        var action = () => { scheduler.RegisterQueue(settings); };
        action.Should().ThrowExactly<ArgumentException>();
    }

    [Test]
    public void ThrowExceptionScheduleJobWithoutQueue()
    {
        var scheduler = new JobScheduler(_builder);

        var settings = new QueueSettings("test", 1);
        var job = new OneTimeQueueJob(TimeSpan.FromSeconds(5))
        {
            QueueId = settings.QueueId
        };
        var action = () => { scheduler.ScheduleJob(job); };
        action.Should().ThrowExactly<ArgumentException>();
    }

    [Test]
    public async Task AddJobToQueue()
    {
        IJobScheduler scheduler = new JobScheduler(_builder);

        var settings = new QueueSettings("test", 1);
        var job = new OneTimeQueueJob(TimeSpan.FromMilliseconds(500))
        {
            QueueId = settings.QueueId
        };

        scheduler.RegisterQueue(settings);
        var queue = scheduler.GetQueue(settings.QueueId);
        scheduler.ScheduleJob(job);
        var jobRunner = queue.RunningJobs.First();
        await jobRunner.WaitForJob();
        job.HasRun.Should().BeTrue();
        queue.RunningJobs.Count().Should().Be(0);
    }

    [Test]
    public async Task AddMultipleJobQueue()
    {
        IJobScheduler scheduler = new JobScheduler(_builder);

        var settings = new QueueSettings("test", 1);
        scheduler.RegisterQueue(settings);
        var queue = scheduler.GetQueue(settings.QueueId);
        for (var i = 0; i < 5; i++)
        {
            var job = new OneTimeQueueJob(TimeSpan.FromMilliseconds(500))
            {
                QueueId = settings.QueueId,
                Key = $"Hello {i}"
            };

            scheduler.ScheduleJob(job);
        }

        var jobRunner = queue.RunningJobs.First();
        await jobRunner.WaitForJob();
        queue.RunningJobs.Count().Should().Be(1);
        queue.QueuedJobs.Count().Should().Be(3);
        await scheduler.StopAsync();
        queue.RunningJobs.Count().Should().Be(0);
        queue.QueuedJobs.Count().Should().Be(0);
    }
}