using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace HostWebApp
{
    public interface IJob
    {
        void Start();
        Task Execute();
    }

    public class Job : IJob
    {
        public void Start()
        {
            //schedules to a date
        }

        public Task Execute()
        {
            return Task.FromResult(0);
        }
    }


    public abstract class ForeverJobBase : IRegisteredObject, IJob
    {
        private readonly object _lock = new object();
        private bool _shuttingDown;

        public string JobName { get; protected set; }

        public int Interval { get; protected set; }

        protected ForeverJobBase(string name, int interval)
        {
            JobName = name;
            Interval = interval;
        }

        public virtual void Start()
        {
            HostingEnvironment.RegisterObject(this);
            AddTask();
        }

        private void AddTask()
        {
            var expiration = DateTime.Now.AddSeconds(Interval);
            HttpRuntime.Cache.Insert(JobName,
                this,
                null,
                expiration,
                Cache.NoSlidingExpiration,
                CacheItemPriority.NotRemovable,
                CacheItemRemoved);

            Trace.TraceInformation("Task '{0}' scheduled to '{1}'", this, expiration);
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}s", JobName, TimeSpan.FromSeconds(Interval));
        }

        private void CacheItemRemoved(string k, object v, CacheItemRemovedReason r)
        {
            lock (_lock)
            {
                if (_shuttingDown)
                {
                    return;
                }
                bool success = false;
                try
                {
                    var result = Execute();
                    result.Wait();
                    success = true;
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Error executing {0}: {1} ", this, ex);
                }
                finally
                {
                    if (success)
                        AddTask();
                }
            }
        }

        public void Stop(bool immediate)
        {
            lock (_lock)
            {
                _shuttingDown = true;
            }
            Trace.TraceInformation("App is shutting down! Task '{0}' removed.", this);
            HostingEnvironment.UnregisterObject(this);
        }

        public virtual System.Threading.Tasks.Task Execute()
        {
            Trace.TraceInformation("{0}: Running scheduled Task '{1}'.", DateTime.Now, this);

            return System.Threading.Tasks.Task.FromResult(0);
        }
    }

    public class ForeverActionJob : ForeverJobBase
    {
        private readonly Func<System.Threading.Tasks.Task> _action;

        public ForeverActionJob(string name, int interval, Func<System.Threading.Tasks.Task> action)
            : base(name, interval)
        {
            _action = action;
        }

        public override System.Threading.Tasks.Task Execute()
        {
            return _action();
        }
    }
}