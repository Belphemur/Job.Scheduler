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
        public Thread MainThread { get; }
        private readonly CancellationTokenSource _cts = new();
        public int Scheduled { get; private set; }

        public MockTaskScheduler()
        {
            MainThread = new Thread(Execute)
            {
                Name = "Mock Thread"
            };
            if (!MainThread.IsAlive)
            {
                MainThread.Start();
            }
        }

        private void Execute()
        {
            try
            {
                foreach (var task in _tasksCollection.GetConsumingEnumerable(_cts.Token))
                {
                    Scheduled++;
                    TryExecuteTask(task);
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
            MainThread.Join();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}