#nullable enable
namespace Job.Scheduler.Utils;

using System;

/// <summary>
/// Provides Debounce() and Throttle() methods.
/// Use these methods to ensure that events aren't handled too frequently.
/// 
/// Throttle() ensures that events are throttled by the interval specified.
/// Only the last event in the interval sequence of events fires.
/// 
/// Debounce() fires an event only after the specified interval has passed
/// in which no other pending event has fired. Only the last event in the
/// sequence is fired.
/// See: https://weblog.west-wind.com/posts/2017/jul/02/debouncing-and-throttling-dispatcher-events 
/// </summary>
public class DebounceDispatcher<T> : IDisposable where T : class
{
    private readonly TimeSpan _interval;
    private readonly Action<T> _action;
    private readonly T? _param;
    private System.Timers.Timer? _timer;
    private DateTime TimerStarted { get; set; } = DateTime.UtcNow.AddYears(-1);

    public DebounceDispatcher(TimeSpan interval, Action<T> action, T? param)
    {
        _interval = interval;
        _action = action;
        _param = param;
    }

    /// <summary>
    /// Debounce an event by resetting the event timeout every time the event is 
    /// fired. The behavior is that the Action passed is fired only after events
    /// stop firing for the given timeout period.
    /// 
    /// Use Debounce when you want events to fire only after events stop firing
    /// after the given interval timeout period.
    /// 
    /// Wrap the logic you would normally use in your event code into
    /// the  Action you pass to this method to debounce the event.
    /// Example: https://gist.github.com/RickStrahl/0519b678f3294e27891f4d4f0608519a
    /// </summary>
    /// <param name="priority">optional priorty for the dispatcher</param>
    /// <param name="disp">optional dispatcher. If not passed or null CurrentDispatcher is used.</param>        
    public void Debounce()
    {
        // kill pending timer and pending ticks
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;

        // timer is recreated for each event and effectively
        // resets the timeout. Action only fires after timeout has fully
        // elapsed without other events firing in between
        _timer = new System.Timers.Timer(_interval.TotalMilliseconds) { AutoReset = false };
        _timer.Elapsed += (sender, args) =>
        {
            if (_timer == null)
                return;

            _timer?.Stop();
            _timer = null;
            _action.Invoke(_param);
        };
        _timer.Start();
    }

    /// <summary>
    /// This method throttles events by allowing only 1 event to fire for the given
    /// timeout period. Only the last event fired is handled - all others are ignored.
    /// Throttle will fire events every timeout ms even if additional events are pending.
    /// 
    /// Use Throttle where you need to ensure that events fire at given intervals.
    /// </summary>
    /// <param name="priority">optional priorty for the dispatcher</param>
    /// <param name="disp">optional dispatcher. If not passed or null CurrentDispatcher is used.</param>
    public void Throttle()
    {
        // kill pending timer and pending ticks
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;


        var curTime = DateTime.UtcNow;
        var interval = _interval.TotalMilliseconds;

        // if timeout is not up yet - adjust timeout to fire 
        // with potentially new Action parameters           
        if (curTime.Subtract(TimerStarted).TotalMilliseconds < interval)
            interval -= (int)curTime.Subtract(TimerStarted).TotalMilliseconds;

        _timer = new System.Timers.Timer(interval);
        _timer.Elapsed += (sender, args) =>
        {
            if (_timer == null)
                return;

            _timer?.Stop();
            _timer = null;
            _action.Invoke(_param);
        };
        _timer.Start();

        TimerStarted = curTime;
    }

    public void Dispose()
    {
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;
    }
}