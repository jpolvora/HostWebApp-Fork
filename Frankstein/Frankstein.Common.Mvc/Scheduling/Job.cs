using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Frankstein.Common.Mvc.Scheduling
{
    public class JobScheduler : IRegisteredObject
    {
        readonly List<IJob> _jobs = new List<IJob>();

        public JobScheduler()
        {
            HostingEnvironment.RegisterObject(this);
        }

        public void Register(IJob job)
        {
            _jobs.Add(job);

        }

        public void Run()
        {
            foreach (var job in _jobs)
            {
                job.Register();
            }
        }

        public void Stop(bool immediate)
        {
            HostingEnvironment.UnregisterObject(this);
            foreach (var job in _jobs)
            {
                job.Unregister();
            }
        }
    }

    public class Job : IJob
    {
        private bool _isShutingDown;

        public void Register()
        {
            

            //schedules to a date

        }

        public void Unregister()
        {

        }

        public virtual Task Execute()
        {
            return Task.FromResult(0);
        }

        public void Stop(bool immediate)
        {

        }
    }
}