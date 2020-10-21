using Job.Scheduler.Scheduler;
using Ninject.Modules;

namespace Job.Scheduler.Bootstrap
{
    public class BootstrapJobScheduler : NinjectModule
    {
        public override void Load()
        {
            Bind<IJobScheduler>().To<JobScheduler>().InSingletonScope();
        }
    }
}