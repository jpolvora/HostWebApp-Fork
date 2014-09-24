using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace Frankstein.Common.Mvc.Jobs
{
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
                System.Web.Caching.Cache.NoSlidingExpiration,
                CacheItemPriority.NotRemovable,
                CacheItemRemoved);

            Trace.TraceInformation("Task '{0}' scheduled to '{1}'", this, expiration);
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}s", JobName, TimeSpan.FromSeconds(Interval));
        }

        private async void CacheItemRemoved(string k, object v, CacheItemRemovedReason r)
        {
            lock (_lock)
            {
                if (_shuttingDown)
                {
                    return;
                }
            }

            bool success = false;
            try
            {
                await Execute();

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

        public void Stop(bool immediate)
        {
            lock (_lock)
            {
                _shuttingDown = true;
            }
            Trace.TraceInformation("App is shutting down! Task '{0}' removed.", this);
            HostingEnvironment.UnregisterObject(this);
        }

        public virtual Task Execute()
        {
            Trace.TraceInformation("{0}: Running scheduled Task '{1}'.", DateTime.Now, this);

            return Task.FromResult(0);
        }
    }
}