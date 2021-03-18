using System;

namespace Job.Scheduler.Job.Data
{
    public struct JobId
    {
        /// <summary>
        /// Unique ID of the job
        /// </summary>
        public Guid UniqueId { get; }

        internal JobId(Guid uniqueId)
        {
            UniqueId = uniqueId;
        }
    }
}