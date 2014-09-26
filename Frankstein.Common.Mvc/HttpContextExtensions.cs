using System;
using System.Web;
using System.Web.Routing;
using System.Web.Security;

namespace Frankstein.Common.Mvc
{
    public static class HttpContextExtensions
    {
        internal const string RouteKey = "__Route";
        public static string GetRouteValue(this HttpContextBase context, string key)
        {
            var route = context.Items[RouteKey] as RouteValueDictionary;
            if (route != null)
            {
                var routeValue = route[key];
                return routeValue != null ? routeValue.ToString() : null;
            }
            return null;
        }

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

        public static string ToPublicUrl(this HttpContextBase context, Uri relativeUri, string scheme = "http")
        {
            var uriBuilder = new UriBuilder
            {
                Host = context.Request.Url.Host,
                Path = "/",
                Port = 80,
                Scheme = scheme,
            };

            if (context.Request.IsLocal)
            {
                uriBuilder.Port = context.Request.Url.Port;
            }

            return new Uri(uriBuilder.Uri, relativeUri).AbsoluteUri;
        }

        public static string ToPublicUrl(this HttpContext context, Uri relativeUri, string scheme = "http")
        {
            var uriBuilder = new UriBuilder
            {
                Host = context.Request.Url.Host,
                Path = "/",
                Port = 80,
                Scheme = scheme,
            };

            if (context.Request.IsLocal)
            {
                uriBuilder.Port = context.Request.Url.Port;
            }

            return new Uri(uriBuilder.Uri, relativeUri).AbsoluteUri;
        }
    }
}