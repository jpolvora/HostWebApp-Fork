using System;
using System.Diagnostics;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace Frankstein.Common.Mvc.Scheduling
{
    public abstract class RecurringJobBase : IRegisteredObject, IJob
    {
        private readonly bool _continueOnError;
        private readonly object _lock = new object();
        private bool _shuttingDown;

        public string JobName { get; protected set; }

        public int Interval { get; protected set; }

        protected RecurringJobBase(string name, int interval, bool continueOnError)
        {
            _continueOnError = continueOnError;
            JobName = name;
            Interval = interval;
        }

        public virtual void Register()
        {
            HostingEnvironment.RegisterObject(this);
            AddTask();
        }

        public void Unregister()
        {
            HostingEnvironment.UnregisterObject(this);

            HttpRuntime.Cache.Remove(JobName);
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

        private void CacheItemRemoved(string k, object v, CacheItemRemovedReason r)
        {
            lock (_lock)
            {
                if (_shuttingDown)
                {
                    return;
                }

                var success = false;
                try
                {
                    Trace.TraceInformation("{0}: Running scheduled Task '{1}'.", DateTime.Now, this);
                    Execute();

                    success = true;
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Error executing {0}: {1} ", this, ex);
                }
                finally
                {
                    if (success || _continueOnError)
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
            Unregister();
        }

        public abstract void Execute();

    }
}