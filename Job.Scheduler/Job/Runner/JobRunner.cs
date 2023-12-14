using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Job.Scheduler.Utils;

namespace Job.Scheduler.Job.Runner
{
    /// <summary>
    /// Base implementation of <see cref="IJobRunner"/>
    /// </summary>
    /// <typeparam name="TJob"></typeparam>
    internal abstract class JobRunner<TJob> : IJobRunner where TJob : IJob
    {
        protected readonly IJobContainerBuilder<TJob> BuilderJobContainer;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _runningTask;
        private Task _runningTaskWithDone;
        private static readonly IRetryAction DefaultFailRule = new NoRetry();
        private readonly Stopwatch _stopwatch = new();
        private readonly Func<IJobRunner,bool, Task> _jobDone;

        [CanBeNull]
        private readonly TaskScheduler _taskScheduler;

        private static readonly ActivitySource _activitySource = new("Job.Scheduler::Runner");

        public Guid UniqueId { get; } = Guid.NewGuid();
        public bool IsRunning => _cancellationTokenSource is { IsCancellationRequested: false } && _runningTaskWithDone is not null;
        public TimeSpan Elapsed => _stopwatch.Elapsed;
        public int Retries { get; private set; }

        public Type JobType => typeof(TJob);
        public virtual string Key => UniqueId.ToString();

        private bool _manuallyStopped = false;


        protected JobRunner(IJobContainerBuilder<TJob> builderJobContainer, Func<IJobRunner, bool, Task> jobDone, [CanBeNull] TaskScheduler taskScheduler)
        {
            BuilderJobContainer = builderJobContainer;
            _jobDone = jobDone;
            _taskScheduler = taskScheduler;
        }

        /// <summary>
        /// Start the job
        /// </summary>
        protected abstract Task StartJobAsync(IJobContainerBuilder<TJob> builderJobContainer, CancellationToken token);

        /// <summary>
        /// Run the job
        /// </summary>
        /// <param name="token">Optional token to sync with it</param>
        /// <returns></returns>
        public void Start(CancellationToken token = default)
        {
            _manuallyStopped = false;
            if (IsRunning)
            {
                throw new InvalidOperationException("Can't start a running job");
            }

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

            _runningTask = _taskScheduler == null
                ? Task.Factory.StartNew( _ => StartJobAsync(BuilderJobContainer, _cancellationTokenSource.Token), null, _cancellationTokenSource.Token).Unwrap()
                : Task.Factory.StartNew(_ => StartJobAsync(BuilderJobContainer, _cancellationTokenSource.Token), null, _cancellationTokenSource.Token, TaskCreationOptions.None, _taskScheduler).Unwrap();

            _runningTaskWithDone = _runningTask.ContinueWith(async _ =>
            {
                if (BuilderJobContainer is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }

                await _jobDone(this, _manuallyStopped);
                _runningTask.Dispose();
                _cancellationTokenSource.Dispose();
            }, CancellationToken.None).Unwrap();
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

            _manuallyStopped = true;
            _cancellationTokenSource.Cancel();
            await Task.WhenAny(TaskUtils.WaitForDelayOrCancellation(TimeSpan.FromMilliseconds(-1), token), _runningTaskWithDone);
            _runningTaskWithDone.Dispose();
            _runningTaskWithDone = null;
            _cancellationTokenSource.Dispose();
            _stopwatch.Stop();
            return _stopwatch.Elapsed;
        }

        Task IJobRunner.WaitForJob()
        {
            return _runningTaskWithDone ?? Task.CompletedTask;
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
            using var maxRuntimeCts = job.MaxRuntime.HasValue ? new CancellationTokenSource(job.MaxRuntime.Value) : new CancellationTokenSource();
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, maxRuntimeCts.Token);
            var runtimeMaxLinkedToken = linkedCts.Token;
            _stopwatch.Restart();
            using var activity = _activitySource.StartActivity("Running Job", ActivityKind.Internal, null, new[]
            {
                new KeyValuePair<string, object>("JobClass", job.GetType().Name),
                new KeyValuePair<string, object>("Retries", Retries)
            });
            try
            {
                await job.ExecuteAsync(runtimeMaxLinkedToken);
                Retries = 0;
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

                    var retryRule = job.FailRule ?? DefaultFailRule;
                    if (retryRule.ShouldRetry(Retries))
                    {
                        var delay = retryRule.GetDelayBetweenRetries(Retries);
                        Retries++;

                        if (delay.HasValue)
                        {
                            await TaskUtils.WaitForDelayOrCancellation(delay.Value, cancellationToken);
                        }

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        await InnerExecuteJob(job, cancellationToken);
                        return;
                    }

                    _cancellationTokenSource.Cancel();
                }
                catch (System.Exception failureException)
                {
                    throw new JobException("Fail to handle failure of job", failureException);
                }
                finally
                {
                    _stopwatch.Stop();
                }
            }
        }
    }
}