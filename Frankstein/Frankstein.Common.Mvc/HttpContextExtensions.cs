using System;
using System.Web;
using System.Web.Security;

namespace Frankstein.Common.Mvc
{
    public static class HttpContextExtensions
    {
        public static string GetRequestId(this HttpContext context)
        {
            var httpContext = context ?? HttpContext.Current;
            if (context == null)
                return Guid.NewGuid().ToString("N");

            return httpContext.Request.GetHashCode().ToString();
        }

        public static string GetRequestId(this HttpContextBase context)
        {
            if (context == null)
                return GetRequestId(HttpContext.Current);

            return context.Request.GetHashCode().ToString();
        }

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