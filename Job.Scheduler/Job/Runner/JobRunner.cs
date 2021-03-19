using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Job.Scheduler.Utils;

namespace Job.Scheduler.Job.Runner
{
    /// <summary>
    /// Base implementation of <see cref="IJobRunner"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class JobRunner<T> : IJobRunner where T : IJob
    {
        private readonly T _job;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _runningTask;
        private Task _runningTaskWithDone;
        private int _retries = 0;
        private static readonly IRetryAction DefaultFailRule = new NoRetry();
        private readonly Stopwatch _stopwatch = new();
        private readonly Func<IJobRunner, Task> _jobDone;

        public Guid UniqueId { get; } = Guid.NewGuid();
        public bool IsRunning => _cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested;
        public TimeSpan Elapsed => _stopwatch.Elapsed;

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

            _stopwatch.Start();

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

            _runningTask = StartJobAsync(_job, _cancellationTokenSource.Token);
            _runningTaskWithDone = _runningTask.ContinueWith(task => _jobDone(this), CancellationToken.None);
        }


        /// <summary>
        /// Stop the job and wait for either the cancellation token or the task to finish
        /// </summary>
        /// <param name="token"></param>
        /// <returns>How long did the job run in total</returns>
        public async Task<TimeSpan> StopAsync(CancellationToken token)
        {
            if (!IsRunning)
            {
                return TimeSpan.Zero;
            }

            _cancellationTokenSource.Cancel();
            await Task.WhenAny(TaskUtils.WaitForDelayOrCancellation(TimeSpan.FromMilliseconds(-1), token), _runningTaskWithDone);
            _stopwatch.Stop();
            return _stopwatch.Elapsed;
        }

        Task IJobRunner.WaitForJob()
        {
            return _runningTaskWithDone;
        }

        /// <summary>
        /// Execute the job
        /// </summary>
        /// <param name="job"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>true if the job should still be running, false if it shouldn't</returns>
        /// <exception cref="JobException"></exception>
        protected async Task InnerExecuteJob(IJob job, CancellationToken cancellationToken)
        {
            using var maxRuntimeCts = _job.MaxRuntime.HasValue ? new CancellationTokenSource(_job.MaxRuntime.Value) : new CancellationTokenSource();
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, maxRuntimeCts.Token);
            var runtimeMaxLinkedToken = linkedCts.Token;
            try
            {
                await job.ExecuteAsync(runtimeMaxLinkedToken);
            }
            catch (System.Exception e)
            {
                try
                {
                    var jobException = new JobException("Job Failed", e);
                    if (e is OperationCanceledException && maxRuntimeCts.IsCancellationRequested)
                    {
                        jobException = new MaxRuntimeJobException("Job reached max runtime", e);
                    }

                    await job.OnFailure(jobException);
                    var retry = job.FailRule ?? DefaultFailRule;
                    if (retry.ShouldRetry(_retries++))
                    {
                        if (retry.DelayBetweenRetries.HasValue)
                        {
                            await TaskUtils.WaitForDelayOrCancellation(retry.DelayBetweenRetries.Value, cancellationToken);
                        }

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        await InnerExecuteJob(job, cancellationToken);
                        return;
                    }

                    _cancellationTokenSource.Cancel();
                    _stopwatch.Stop();
                }
                catch (System.Exception failureException)
                {
                    throw new JobException("Fail to handle failure of job", failureException);
                }
            }
        }

        /// <summary>
        /// Dispose runner
        /// </summary>
        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
            _runningTask.Dispose();
            _runningTaskWithDone.Dispose();
        }
    }
}