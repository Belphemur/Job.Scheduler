using System;
using System.Collections.Generic;
using System.Linq;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Runner;

namespace Job.Scheduler.Builder
{
    public class JobRunnerBuilder : IJobRunnerBuilder
    {
        private readonly Dictionary<Type, Type> _jobTypeToRunnerTypeDictionary;
        private readonly Dictionary<Type, Type> _jobToRunner = new Dictionary<Type, Type>();

        public JobRunnerBuilder()
        {
            var jobRunnerType = typeof(IJobRunner);
            _jobTypeToRunnerTypeDictionary = jobRunnerType.Assembly.GetTypes()
                                                          .Where(type => type.IsClass && !type.IsAbstract)
                                                          .Where(type => jobRunnerType.IsAssignableFrom(type) && type.BaseType?.IsAbstract == true)
                                                          .ToDictionary(type => type.BaseType.GetGenericArguments().First());
        }

        /// <summary>
        /// Build a Job runner for the given job
        /// </summary>
        public IJobRunner Build(IJob job)
        {
            var mainTypeJob = job.GetType();
            if (!_jobToRunner.TryGetValue(mainTypeJob, out var runner))
            {
                var typeOfJob = mainTypeJob.GetInterfaces().Intersect(_jobTypeToRunnerTypeDictionary.Keys).First();
                runner = _jobTypeToRunnerTypeDictionary[typeOfJob];
                _jobToRunner.Add(mainTypeJob, runner);
            }

            return (IJobRunner) Activator.CreateInstance(runner, job);
        }
    }
}