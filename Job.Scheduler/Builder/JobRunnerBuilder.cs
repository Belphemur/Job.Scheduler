using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Runner;

namespace Job.Scheduler.Builder
{
    /// <summary>
    /// Take care of building the runner for different type of <see cref="IJob"/>
    /// </summary>
    public class JobRunnerBuilder : IJobRunnerBuilder
    {
        private readonly Dictionary<Type, Type> _jobTypeToRunnerTypeDictionary;
        private readonly ConcurrentDictionary<Type, Type> _jobToRunner = new();

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
        public IJobRunner Build<TJob>(IJobContainerBuilder<TJob> builder, Func<IJobRunner, Task> jobDone, TaskScheduler taskScheduler) where TJob : IJob
        {
            var mainTypeJob = builder.JobType;

            _jobTypeToRunnerTypeDictionary.TryGetValue(mainTypeJob, out var runner);

            if (runner == null && !_jobToRunner.TryGetValue(mainTypeJob, out runner))
            {
                var typeOfJob = mainTypeJob.GetInterfaces().Intersect(_jobTypeToRunnerTypeDictionary.Keys).First();
                runner = _jobTypeToRunnerTypeDictionary[typeOfJob];
                _jobToRunner.TryAdd(mainTypeJob, runner);
            }

            return (IJobRunner)Activator.CreateInstance(runner, builder, jobDone, taskScheduler);
        }
    }
}