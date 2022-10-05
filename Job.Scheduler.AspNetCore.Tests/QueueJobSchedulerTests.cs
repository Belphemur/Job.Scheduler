using FluentAssertions;
using Job.Scheduler.AspNetCore.Background;
using Job.Scheduler.AspNetCore.Builder;
using Job.Scheduler.AspNetCore.Extensions;
using Job.Scheduler.AspNetCore.Tests.Mock;
using Job.Scheduler.Builder;
using Job.Scheduler.Queue;
using Job.Scheduler.Scheduler;
using Job.Scheduler.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Job.Scheduler.AspNetCore.Tests;

[TestFixture]
public class QueueJobSchedulerTests
{
    private ServiceCollection _services;
    private QueueSettings _settings;
    private JobSchedulerHostedService _jobSchedulerHostedService;
    private ServiceProvider _provider;

    [OneTimeSetUp]
    public async Task Setup()
    {
        _services = new ServiceCollection();
        _settings = new QueueSettings("test", 1);
        _services.AddJobScheduler(config => { config.RegisterQueue(_settings); })
                 .AddJob<OneTimeQueueJob>();

        _provider = await GetProvider();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _jobSchedulerHostedService.StopAsync(CancellationToken.None);
        await _provider.DisposeAsync();
    }

    [Test]
    public async Task AddJobToQueue()
    {
        var builder = _provider.GetRequiredService<IJobBuilder>();
        var runState = new OneTimeQueueJob.RunStateInfo();
        var job = builder.Create<OneTimeQueueJob>()
                         .Configure(queueJob =>
                         {
                             queueJob.QueueId = _settings.QueueId;
                             queueJob.WaitTime = TimeSpan.FromMilliseconds(500);
                             queueJob.RunState = runState;
                         })
                         .Build();

        var scheduler = _provider.GetRequiredService<IJobScheduler>();

        var queue = scheduler.GetQueue(_settings.QueueId);
        scheduler.ScheduleJob(job);
        var jobRunner = queue.RunningJobs.First();
        await jobRunner.WaitForJob();
        runState.HasRun.Should().BeTrue();
        queue.RunningJobs.Count().Should().Be(0);
    }

    private async Task<ServiceProvider> GetProvider()
    {
        var provider = _services.BuildServiceProvider();
        var hostedServices = provider.GetRequiredService<IEnumerable<IHostedService>>();
        var hostedService = hostedServices.First();

        _jobSchedulerHostedService = (JobSchedulerHostedService)hostedService;
        await _jobSchedulerHostedService.StartAsync(CancellationToken.None);
        return provider;
    }

    [Test]
    public async Task AddMultipleJobQueue()
    {
        var builder = _provider.GetRequiredService<IJobBuilder>();
        var runState = new OneTimeQueueJob.RunStateInfo();

        var scheduler = _provider.GetRequiredService<IJobScheduler>();

        var queue = scheduler.GetQueue(_settings.QueueId);
        for (var i = 0; i < 5; i++)
        {
            var job = builder.Create<OneTimeQueueJob>()
                             .Configure(queueJob =>
                             {
                                 queueJob.QueueId = _settings.QueueId;
                                 queueJob.WaitTime = TimeSpan.FromMilliseconds(500);
                                 queueJob.RunState = runState;
                                 queueJob.Key = $"Hello {i}";
                             })
                             .Build();

            scheduler.ScheduleJob(job);
        }

        var jobRunner = queue.RunningJobs.First();
        await jobRunner.WaitForJob();
        await TaskUtils.WaitForDelayOrCancellation(TimeSpan.FromMilliseconds(100), CancellationToken.None);
        queue.RunningJobs.Count().Should().Be(1);
        queue.QueuedJobsCount.Should().Be(3);
        await scheduler.StopAsync();
        queue.RunningJobs.Count().Should().Be(0);
        queue.QueuedJobsCount.Should().Be(0);
    }
}