namespace Job.Scheduler.Queue;

/// <summary>
/// Settings of the queue
/// </summary>
/// <param name="QueueId">UniqueId of the queue</param>
/// <param name="MaxConcurrency">Max number of job that can be run concurrently on the queue</param>
public record QueueSettings(string QueueId, int MaxConcurrency);