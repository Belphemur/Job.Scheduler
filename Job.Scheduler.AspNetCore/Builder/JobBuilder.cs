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
        public IContainerJob<T> Build()
        {
            return new ScopedJobContainer<T>(_serviceScope, _configurators);
        }
    }

    internal class ScopedJobContainer<TJob> : IContainerJob<TJob> where TJob : IJob
    {
        private readonly IServiceScope _serviceScope;
        private readonly List<Action<TJob>> _configurators;

        public ScopedJobContainer(IServiceScope serviceScope, List<Action<TJob>> configurators)
        {
            _serviceScope = serviceScope;
            _configurators = configurators;
            var job = BuildJob();
            Key = job is HasKey keyed ? keyed.Key : Guid.NewGuid().ToString();
            QueueId = job is IQueueJob queueJob ? queueJob.QueueId : null;
        }

        public Task OnCompletedAsync(CancellationToken token)
        {
            _serviceScope.Dispose();
            return Task.CompletedTask;
        }

        public TJob BuildJob()
        {
            var job = _serviceScope.ServiceProvider.GetRequiredService<TJob>();
            foreach (var action in _configurators)
            {
                action(job);
            }

            return job;
        }

        public Type JobType => typeof(TJob);
        public string Key { get; }
        public string? QueueId { get; }
    }

    public Container<T> Create<T>() where T : IJob => new(_serviceProvider.CreateScope());
}