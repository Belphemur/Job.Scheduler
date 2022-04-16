using Job.Scheduler.Job;
using Microsoft.Extensions.DependencyInjection;

namespace Job.Scheduler.AspNetCore.Builder;

public class JobBuilder : IJobBuilder
{
    private readonly IServiceProvider _serviceProvider;

    public JobBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public class Container<T> where T : IJob
    {
        private readonly IServiceScope _serviceScope;
        private readonly List<Action<T>> _configurators = new();

        public Container(IServiceScope serviceScope)
        {
            _serviceScope = serviceScope;
        }

        /// <summary>
        /// Configure the job
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public Container<T> Configure(Action<T> configuration)
        {
            _configurators.Add(configuration);
            return this;
        }

        /// <summary>
        /// Build the <see cref="IJob"/>
        /// </summary>
        /// <returns></returns>
        public IContainerJob Build()
        {
            var job = _serviceScope.ServiceProvider.GetRequiredService<T>();
            foreach (var action in _configurators)
            {
                action(job);
            }

            return new ScopedJobContainer(_serviceScope, job);
        }
    }

    private class ScopedJobContainer : IContainerJob
    {
        private readonly IServiceScope _serviceScope;
        public IJob Job { get; }

        public ScopedJobContainer(IServiceScope serviceScope, IJob job)
        {
            _serviceScope = serviceScope;
            Job = job;
        }

        public Task OnCompletedAsync(CancellationToken token)
        {
            _serviceScope.Dispose();
            return Task.CompletedTask;
        }
    }

    public Container<T> Create<T>() where T : IJob => new(_serviceProvider.CreateScope());
}