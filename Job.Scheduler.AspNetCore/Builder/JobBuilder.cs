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
        private readonly IServiceProvider _serviceProvider;
        private readonly List<Action<T>> _configurators = new();

        public Container(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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
        public IJobContainerBuilder<T> Build()
        {
            return new ScopedBuilderJobContainer<T>(_serviceProvider, _configurators);
        }
    }

    internal class ScopedJobContainer<TJob> : IJobContainer<TJob> where TJob : IJob
    {
        private readonly IServiceScope _serviceScope;
        private bool _isDisposed;

        public ScopedJobContainer(TJob job, IServiceScope serviceScope)
        {
            _serviceScope = serviceScope;
            Job = job;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _serviceScope.Dispose();
            _isDisposed = true;
        }

        public TJob Job { get; }
    }

    internal class ScopedBuilderJobContainer<TJob> : IJobContainerBuilder<TJob> where TJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly List<Action<TJob>> _configurators;
        private readonly List<IJobContainer<TJob>> _jobBuilt = new();

        public ScopedBuilderJobContainer(IServiceProvider serviceProvider, List<Action<TJob>> configurators)
        {
            _serviceProvider = serviceProvider;
            _configurators = configurators;
            using var jobContainer = BuildJob();
            var job = jobContainer.Job;
            Key = job is HasKey keyed ? keyed.Key : Guid.NewGuid().ToString();
            QueueId = job is IQueueJob queueJob ? queueJob.QueueId : null;
        }

        public Task OnCompletedAsync(CancellationToken token)
        {
            foreach (var container in _jobBuilt)
            {
                container.Dispose();
            }
            _configurators.Clear();
            _jobBuilt.Clear();
            return Task.CompletedTask;
        }

        public IJobContainer<TJob> BuildJob()
        {
            var serviceScope = _serviceProvider.CreateScope();
            var job = serviceScope.ServiceProvider.GetRequiredService<TJob>();
            foreach (var action in _configurators)
            {
                action(job);
            }

            var scopedJobContainer = new ScopedJobContainer<TJob>(job, serviceScope);
            _jobBuilt.Add(scopedJobContainer);
            return scopedJobContainer;
        }

        public Type JobType => typeof(TJob);
        public string Key { get; }
        public string? QueueId { get; }
    }

    public Container<T> Create<T>() where T : IJob => new(_serviceProvider);
}