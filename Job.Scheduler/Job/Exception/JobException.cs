using Job.Scheduler.Scheduler;

namespace Job.Scheduler.Job.Exception
{
    /// <summary>
    /// Wrapped exception of the <see cref="IJobScheduler"/>
    /// </summary>
    public class JobException : System.Exception
    {
        public JobException(string message, System.Exception exception) : base(message, exception) {}
    }
}