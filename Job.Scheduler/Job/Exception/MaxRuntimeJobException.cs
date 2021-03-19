namespace Job.Scheduler.Job.Exception
{
    /// <summary>
    /// Thrown when a Job took longer than it's max runtime
    /// </summary>
    public class MaxRuntimeJobException : JobException
    {
        public MaxRuntimeJobException(string message, System.Exception exception) : base(message, exception)
        {
        }
    }
}