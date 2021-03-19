using System;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Job.Scheduler.Utils;

namespace Job.Scheduler.Job.Runner
{
    internal abstract class JobRunner<T> : IJobRunner where T : IJob
    {
        private readonly T _job;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _runningTask;
        private int _retries = 0;
        private static readonly NoRetry DefaultFailRule = new NoRetry();

        /// <summary>
        /// Unique ID of the job runner
        /// </summary>
        public Guid UniqueId { get; } = Guid.NewGuid();

        /// <summary>
        /// Run when the job is Done
        /// </summary>
        private readonly Func<IJobRunner, Task> _jobDone;

        /// <summary>
        /// Is the job still running
        /// </summary>
        public bool IsRunning => _cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested;

        protected JobRunner(T job, Func<IJobRunner, Task> jobDone)
        {
            _job = job;
            _jobDone = jobDone;
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
            if (!IsRunning)
            {
                return;
            }

            _cancellationTokenSource.Cancel();
            await Task.WhenAny(TaskUtils.WaitForDelayOrCancellation(TimeSpan.FromMilliseconds(-1), token), _runningTask);
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
                await _jobDone(this);
            }
        }

        /// <summary>
        /// Execute the job
        /// </summary>
        /// <param name="job"></param>
        /// <param name="token"></param>
        /// <returns>true if the job should still be running, false if it shouldn't</returns>
        /// <exception cref="JobException"></exception>
        protected async Task ExecuteJob(IJob job, CancellationToken token)
        {
            try
            {
                await job.ExecuteAsync(token);
            }
            catch (System.Exception e)
            {
                try
                {
                    await job.OnFailure(new JobException("Job Failed", e));
                    var retry = job.FailRule ?? DefaultFailRule;
                    if (retry.ShouldRetry(_retries++))
                    {
                        if (retry.DelayBetweenRetries.HasValue)
                        {
                            await TaskUtils.WaitForDelayOrCancellation(retry.DelayBetweenRetries.Value, token);
                        }

                        if (token.IsCancellationRequested)
                        {
                            return;
                        }

                        await ExecuteJob(job, token);
                        return;
                    }

                    await StopAsync(default);

                }
                catch (System.Exception failureException)
                {
                    throw new JobException("Fail to handle failure of job", failureException);
                }
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
            _runningTask.Dispose();
        }
    }
}