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
            if (_wasInit)
                return false;

            lock (Lock)
            {
                if (_wasInit)
                    return false;

                var context = HttpContext.Current;

                if (context == null || context.Request == null)
                    throw new Exception("HttpContext not available at this moment.");

                HostUrl = context.ToPublicUrl(new Uri("/", UriKind.Relative));

                _wasInit = true;
                InvokeOnFirstRequest(context.ApplicationInstance);
                return true;
            }
        }
    }
}