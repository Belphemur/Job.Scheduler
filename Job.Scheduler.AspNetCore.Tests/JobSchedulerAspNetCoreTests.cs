using FluentAssertions;
using Job.Scheduler.AspNetCore.Background;
using Job.Scheduler.AspNetCore.Builder;
using Job.Scheduler.AspNetCore.Configuration;
using Job.Scheduler.AspNetCore.Extensions;
using Job.Scheduler.AspNetCore.Tests.Mock;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Job.Scheduler.AspNetCore.Tests;

public class Tests
{
    private ServiceCollection _services;

    [SetUp]
    public void Setup()
    {
        _services = new ServiceCollection();
    }

    [Test]
    public void TestJobBuilder()
    {
        _services.AddJob<OneTimeJob>();
        _services.AddSingleton<IJobBuilder, JobBuilder>();

        var serviceProvider = _services.BuildServiceProvider();

        var builder = serviceProvider.GetRequiredService<IJobBuilder>();
        var container = builder.Create<OneTimeJob>()
                               .Build();
        container.Should().BeOfType<JobBuilder.ScopedBuilderJobContainer<OneTimeJob>>();

        var job = container.BuildJob().Job;

        job.Should().BeOfType<OneTimeJob>();
    }

    [Test]
    public void TestJobConfig()
    {
        _services.AddJob<OneTimeJob>();
        _services.AddJobScheduler(config => { config.AddStartupJob(builder => builder.Create<OneTimeJob>().Build()); });
        var container = _services.BuildServiceProvider();

        var config = container.GetRequiredService<JobSchedulerStartupConfig>();
        config.OneTimeJobs.Should().ContainSingle(job => job.JobType == typeof(OneTimeJob));
    }

    [Test]
    public async Task TestJobHostedService()
    {
        var run = new HasRunJob.Runstate();
        _services.AddJobScheduler(config =>
        {
            config.AddStartupJob(builder => builder.Create<HasRunJob>()
                                                   .Configure(runJob => runJob.Run = run)
                                                   .Build());
        });
        _services.AddJob<HasRunJob>();

        var container = _services.BuildServiceProvider();

        var hostedServices = container.GetRequiredService<IEnumerable<IHostedService>>();
        var hostedService = hostedServices.First();

        hostedService.Should().BeOfType<JobSchedulerHostedService>();

        await hostedService.StartAsync(CancellationToken.None);

        await Task.Delay(TimeSpan.FromMilliseconds(10));

        await hostedService.StopAsync(CancellationToken.None);

        run.HasRun.Should().BeTrue("Job has run part of startup");
    }
}