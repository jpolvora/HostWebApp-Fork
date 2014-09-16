using System;
using System.Web;
using System.Web.Security;

namespace Frankstein.Common.Mvc
{
    public static class HttpContextExtensions
    {
        public static void RedirectSafeToDefault(this HttpContext context, string extraQuery = null)
        {
            string url = FormsAuthentication.DefaultUrl;    
            if (!string.IsNullOrEmpty(extraQuery))
                url += "?" + extraQuery;
            RedirectSafe(context, url);
        }

        public static void RedirectSafe(this HttpContext context, string url)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            context.Response.Redirect(url, false);
            context.Response.SuppressContent = true;
            context.ApplicationInstance.CompleteRequest();
        }

        public static void RedirectSafeToDefault(this HttpContextBase context, string extraQuery = null)
        {
            string url = FormsAuthentication.DefaultUrl;
            if (!string.IsNullOrEmpty(extraQuery))
                url += "?" + extraQuery;
            RedirectSafe(context, url);
        }

        public static void RedirectSafe(this HttpContextBase context, string url)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");

            context.Response.Redirect(url, false);
            context.Response.SuppressContent = true;
            context.ApplicationInstance.CompleteRequest();
        }
    }
}