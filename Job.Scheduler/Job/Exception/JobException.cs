namespace Job.Scheduler.Job.Exception
{
    public class JobException : System.Exception
    {
        public JobException(string message, System.Exception exception) : base(message, exception) {}
    }
}