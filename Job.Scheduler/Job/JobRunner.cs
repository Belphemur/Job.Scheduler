using System;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job.Exception;

namespace Job.Scheduler.Job
{
    internal class JobRunner
    {
        private readonly IJob                    _job;
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

        public JobRunner(IJob job)
        {
            _job = job;
        }


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
            if (_job is IRecurringJob recurringJob)
            {
                _runningTask = StartRecurringJobAsync(recurringJob, _cancellationTokenSource.Token);
            }
            else if (_job is IDelayedJob delayedJob)
            {
                _runningTask = StartDelayedJobAsync(delayedJob, _cancellationTokenSource.Token);
            }
            else
            {
                _runningTask = StartOneTimeJobAsync(_job, _cancellationTokenSource.Token);
            }
        }

        private Task StartDelayedJobAsync(IDelayedJob delayedJob, CancellationToken token)
        {
            return RunAsyncWithDone(async cancellationToken =>
            {
                await Task.Delay(delayedJob.Delay, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await ExecuteJob(delayedJob, cancellationToken);
            }, token);
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


        private Task StartRecurringJobAsync(IRecurringJob job, CancellationToken token)
        {
            return RunAsyncWithDone(async cancellationToken =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (!await ExecuteJob(job, cancellationToken)) break;

                    await Task.Delay(job.Delay, cancellationToken);
                }
            }, token);
        }

        private Task StartOneTimeJobAsync(IJob job, CancellationToken token)
        {
            return RunAsyncWithDone(cancellationToken => ExecuteJob(job, cancellationToken), token);
        }

        /// <summary>
        /// Set the runner as done after running the given code
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
        private async Task<bool> ExecuteJob(IJob job, CancellationToken token)
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