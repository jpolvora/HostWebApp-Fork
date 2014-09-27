using System;
using System.Diagnostics;
using System.Web;

namespace Frankstein.Common.Mvc
{
    public static class RequestCheck
    {
        private static readonly object Lock = new object();
        private static bool _wasInit;
        public static string HostUrl { get; private set; }

        public static event EventHandler FirstRequest = delegate { };

        private static void InvokeOnFirstRequest(HttpApplication application)
        {
            Trace.TraceInformation("[RequestCheck]: Firing FirstRequest event");
            var handler = FirstRequest;
            if (handler != null)
            {
                FirstRequest(application, EventArgs.Empty);
            }
        }

        public static bool IsFirstRequest()
        {
            UpdateHost();

            if (_wasInit)
                return false;

            lock (Lock)
            {
                if (_wasInit)
                    return false;

                _wasInit = true;

                var context = HttpContext.Current;
                if (context == null)
                    return false;

                InvokeOnFirstRequest(context.ApplicationInstance);
                return true;
            }
        }

        private static void UpdateHost()
        {
            var context = HttpContext.Current;

            if (context == null || context.Request == null)
                return;

            HostUrl = context.ToPublicUrl(new Uri("/", UriKind.Relative));
        }
    }
}