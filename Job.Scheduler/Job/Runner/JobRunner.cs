using System;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job.Exception;

namespace Job.Scheduler.Job.Runner
{
    internal abstract class JobRunner<T> : IJobRunner where T : IJob
    {
        private readonly T                       _job;
        private          CancellationTokenSource _cancellationTokenSource;
        private          Task                    _runningTask;
        private volatile bool                    _isDone;
        public event EventHandler                JobDone;

        /// <summary>
        /// Unique ID of the job runner
        /// </summary>
        public Guid UniqueId { get; } = Guid.NewGuid();

        /// <summary>
        /// Has the job finished running
        /// </summary>
        public bool IsDone
        {
            get => _isDone;
            private set
            {
                _isDone = value;
                if (_isDone)
                {
                    JobDone?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Is the job still running
        /// </summary>
        public bool IsRunning => _cancellationTokenSource != null && !IsDone;

        public JobRunner(T job)
        {
            _job = job;
        }

        /// <summary>
        /// Start the job
        /// </summary>
        protected abstract Task StartJobAsync(T job, CancellationToken token);

        /// <summary>
        /// Run the job
        /// </summary>
        /// <param name="token">Optional token to sync with it</param>
        /// <returns></returns>
        public void Start(CancellationToken token = default)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Can't start a running job");
            }

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

            _runningTask = RunAsyncWithDone(cancellationToken => StartJobAsync(_job, cancellationToken), token);
        }

        /// <summary>
        /// Stop the task and wait for it to terminate
        /// </summary>
        /// <returns></returns>
        public async Task StopAsync()
        {
            _cancellationTokenSource.Cancel();
            await _runningTask;
            _cancellationTokenSource.Dispose();
        }

        /// <summary>
        /// Set the runner as done after running the given <see cref="task"/>
        /// </summary>
        /// <param name="task"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private Task RunAsyncWithDone(Func<CancellationToken, Task> task, CancellationToken token)
        {
            try
            {
                return token.IsCancellationRequested ? Task.CompletedTask : task(token);
            }
            finally
            {
                _isDone = true;
            }
        }

        /// <summary>
        /// Execute the job
        /// </summary>
        /// <param name="job"></param>
        /// <param name="token"></param>
        /// <returns>true if the job should still be running, false if it shouldn't</returns>
        /// <exception cref="JobException"></exception>
        protected async Task<bool> ExecuteJob(IJob job, CancellationToken token)
        {
            try
            {
                await job.ExecuteAsync(token);
            }
            catch (System.Exception e)
            {
                try
                {
                    if (!await job.OnFailure(new JobException("Job Failed", e)))
                    {
                        return false;
                    }
                }
                catch (System.Exception failureException)
                {
                    throw new JobException("Fail to handle failure of job", failureException);
                }
            }

            return true;
        }
    }
}