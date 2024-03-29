﻿using Job.Scheduler.AspNetCore.Background;
using Job.Scheduler.AspNetCore.Builder;
using Job.Scheduler.AspNetCore.Configuration;
using Job.Scheduler.Builder;
using Job.Scheduler.Job;
using Job.Scheduler.Scheduler;
using Microsoft.Extensions.DependencyInjection;

namespace Job.Scheduler.AspNetCore.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register the job scheduler
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config">Used to setup Startup Jobs</param>
    /// <returns></returns>
    public static IServiceCollection AddJobScheduler(this IServiceCollection services, Action<JobSchedulerStartupConfig>? config = null)
    {
        services.AddSingleton<IJobRunnerBuilder, JobRunnerBuilder>();
        services.AddSingleton<IJobScheduler, JobScheduler>();
        services.AddSingleton(provider =>
        {
            var startupConfig = new JobSchedulerStartupConfig(provider.GetRequiredService<IJobBuilder>());
            config?.Invoke(startupConfig);
            return startupConfig;
        });
        services.AddHostedService<JobSchedulerHostedService>();
        services.AddSingleton<IJobBuilder, JobBuilder>();

        return services;
    }

    /// <summary>
    /// Register the job in the service collection
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddJob<T>(this IServiceCollection services) where T : IJob
    {
        services.AddTransient(typeof(T));
        return services;
    }
}