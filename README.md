﻿# Job Scheduler

[![.NET 5.0](https://github.com/Belphemur/Job.Scheduler/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Belphemur/Job.Scheduler/actions/workflows/dotnet.yml)

A simple job scheduling library relying on the async/await pattern in C#.

## Type of Jobs

### One Time Job

By implementing the `IJob` interface you tell the scheduler that you just want this job to be executed once and directly
upon being scheduled.

### Recurring Job

By implementing the `IRecurringJob` the scheduler will run indefinitely your job with the given delay between execution.

### Delayed Job

By implementing the `IDelayedJob` you tell the scheduler to wait a delay before executing your job.

### Debounce Job

By implementing the `IDebounceJob` you tell the scheduler to only run the latest encounter of the job sharing the same key.


## Usage

I advise you to use a Dependency Injection (DI) engine (like SimpleInjector) to register the `JobRunnerBuilder`and `JobScheduler` as singleton.

### Example:

```c#
public class MyJob : IRecurringJob
{
    //Set the retry rule in case of failure of the job, in this case we want
    //to retry the job 3 times
    //Works for any type of job
    public IRetryAction FailRule { get; } = new RetryNTimes(3);
   
    //Optional MaxRuntime for the job before its canncellationToke get cancelled
    //Keep in mind, this only cancel the token, we have no clean way of stopping a running task
    //then cancelling the token.
    public TimeSpan? MaxRuntime { get; } = TimeSpan.FromSeconds(5);


    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        //Your complex recurring code, here pretty simple
        await Console.Out.WriteLineAsync("Hello World");
    }

    public Task<IRetryAction> OnFailure(JobException exception)
    {
//Any exception that occured when executing your job will be wrapped in a JobException, check the InnerException
//for you to be able to handle a failure without breaking your application neither needed a try/catch in ExecuteAsync


        return Task.CompletedTask;
    }
//This job will run every 15 seconds

    public TimeSpan Delay { get; } = TimeSpan.FromSeconds(15);
}

var builder = new JobRunnerBuilder();
var scheduler = new JobScheduler(builder);

//If you have already a cancellation token that you want to be used for stopping your job, you can pass it as second param
scheduler.Start(new MyJob());

//At the end of your application, you can ask the Scheduler to gracefully stop the running jobs and wait for them to stop.
//You can also pass a cancellationToken to force a non graceful cancellation of the jobs.
await scheduler.StopAsync();
```

### Advanced
You can also use your own TaskScheduler. It's useful if you want to control in which thread your task is run.
```c#
var builder = new JobRunnerBuilder();
var scheduler = new JobScheduler(builder);
var taskScheduler = new MyTaskScheduler();

// This way, this specific instance of the job will be run in your defined task scheduler
scheduler.Start(new MyJob(), CancellationToken.None, taskScheduler);
```