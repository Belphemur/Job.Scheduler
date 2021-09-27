using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Job.Scheduler.Tests.Mocks
{
    public class MockTaskScheduler : TaskScheduler, IDisposable
    {
        private readonly BlockingCollection<Task> _tasksCollection = new();
        private readonly Thread _mainThread;
        private readonly CancellationTokenSource _cts = new();
        public int Count { get; private set; }

        public MockTaskScheduler()
        {
            _mainThread = new Thread(Execute);
            if (!_mainThread.IsAlive)
            {
                _mainThread.Start();
            }
        }

        private void Execute()
        {
            try
            {
                foreach (var task in _tasksCollection.GetConsumingEnumerable(_cts.Token))
                {
                    TryExecuteTask(task);
                    Count++;
                }
            }
            catch (OperationCanceledException)
            {
                //ignored, just stop
            }
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return _tasksCollection.ToArray();
        }

        protected override void QueueTask(Task task)
        {
            if (task != null)
                _tasksCollection.Add(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }

        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            _tasksCollection.CompleteAdding();
            _cts.Cancel();
            _cts.Dispose();
            _tasksCollection.Dispose();
            _mainThread.Join();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}