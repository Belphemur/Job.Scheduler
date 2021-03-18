using System;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job.Exception;
using Job.Scheduler.Utils;

namespace Job.Scheduler.Job.Runner
{
    internal abstract class JobRunner<T> : IJobRunner where T : IJob
    {
        private readonly T _job;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _runningTask;
        private volatile bool _isDone;

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
            private set => _isDone = value;
        }
        /// <summary>
        /// Run when the job is Done
        /// </summary>
        public Func<IJobRunner,Task> JobDone { get; }

        /// <summary>
        /// Is the job still running
        /// </summary>
        public bool IsRunning => _cancellationTokenSource != null && !IsDone;

        public JobRunner(T job, Func<IJobRunner, Task> jobDone)
        {
            _job = job;
            JobDone = jobDone;
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

            _runningTask = RunAsyncWithDone(() => StartJobAsync(_job, _cancellationTokenSource.Token));
        }


        public async Task StopAsync(CancellationToken token)
        {
            _cancellationTokenSource.Cancel();
            await Task.WhenAny(TaskUtils.WaitForDelayOrCancellation(TimeSpan.FromMilliseconds(-1), token), _runningTask);
            _cancellationTokenSource.Dispose();
        }

        /// <summary>
        /// Set the runner as done after running the given <see cref="task"/>
        /// </summary>
        private async Task RunAsyncWithDone(Func<Task> task)
        {
            try
            {
                await task();
            }
            finally
            {
                IsDone = true;
                await JobDone(this);
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