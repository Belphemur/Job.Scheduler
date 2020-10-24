using System;
using System.Collections.Generic;
using System.Linq;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Runner;

namespace Job.Scheduler.Bootstrap
{
    internal class JobRunnerBuilder
    {
        private readonly Dictionary<Type, Type> _jobTypeToRunnerTypeDictionary;

        public JobRunnerBuilder()
        {
            var jobRunnerType = typeof(IJobRunner);
            _jobTypeToRunnerTypeDictionary = AppDomain.CurrentDomain.GetAssemblies()
                                                      .SelectMany(s => s.GetTypes())
                                                      .Where(t => t.IsGenericType)
                                                      .Where(p => jobRunnerType.IsAssignableFrom(p))
                                                      .ToDictionary(type => type.GetGenericArguments().First());
        }

        /// <summary>
        /// Build a Job runner for the given job
        /// </summary>
        public IJobRunner Build(IJob job)
        {
            var typeOfJob    = job.GetType();
            var typeOfRunner = _jobTypeToRunnerTypeDictionary[typeOfJob].MakeGenericType(typeOfJob);
            return (IJobRunner) Activator.CreateInstance(typeOfRunner, job);
        }
    }
}