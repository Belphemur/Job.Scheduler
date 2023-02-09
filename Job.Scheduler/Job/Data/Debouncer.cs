using System;
using Job.Scheduler.Job.Runner;
using Job.Scheduler.Utils;

namespace Job.Scheduler.Job.Data;

internal class Debouncer : IDisposable
{
    public DebounceJobRunner JobRunner { get; private set; }
    private readonly DebounceDispatcher<object> _debouncer;
    
    public Debouncer(IDebounceJob job, DebounceJobRunner jobRunner)
    {
        JobRunner = jobRunner;
        _debouncer = new DebounceDispatcher<object>(job.DebounceTime, _ =>
        {
            JobRunner.Start();
        }, null);
    }


    public void Debounce(DebounceJobRunner jobRunner)
    {
        var stoppingTask = JobRunner.StopAsync(default);
        _debouncer.Debounce();
        JobRunner = jobRunner;
        stoppingTask.GetAwaiter().GetResult();
    }

    public void Start()
    {
        _debouncer.Debounce();
    }

    public void Dispose()
    {
        _debouncer?.Dispose();
    }
}