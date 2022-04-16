using System.Threading;
using System.Threading.Tasks;

namespace Job.Scheduler.Tests.Mocks
{
    public class ThreadJob : OneTimeJob
    {
        public Thread InitThread { get; }
        public Thread RunThread { get; private set; }

        public ThreadJob(Thread initThread)
        {
            InitThread = initThread;
        }
        public override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            RunThread = Thread.CurrentThread;
            return base.ExecuteAsync(cancellationToken);
        }
    }
}