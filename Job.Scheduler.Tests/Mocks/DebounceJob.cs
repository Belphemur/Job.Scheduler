using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;

namespace Job.Scheduler.Tests.Mocks
{
    public class DebounceJob : IDebounceJob
    {
        public IRetryAction FailRule { get; } = new NoRetry();
        public TimeSpan? MaxRuntime { get; }

        private readonly List<string> _list;
        private readonly int _id;

        public DebounceJob(List<string> list, string key, int id)
        {
            _list = list;
            _id = id;
            Key = key;
        }

        public virtual Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _list.Add(Key + _id);
            return Task.CompletedTask;
        }

        public Task OnFailure(JobException exception)
        {
            return Task.CompletedTask;
        }

        public string Key { get; }
        public TimeSpan DebounceTime { get; } = TimeSpan.FromMilliseconds(100);
    }
}