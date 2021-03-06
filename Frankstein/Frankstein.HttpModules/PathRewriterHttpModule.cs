using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;
using Frankstein.Common;
using Frankstein.Common.Configuration;

namespace Frankstein.HttpModules
{
    public class PathRewriterHttpModule : IHttpModule
    {
        private static readonly string _rewriteBasePath;
        private static readonly string[] _excludePaths;

        static PathRewriterHttpModule()
        {
            _rewriteBasePath = BootstrapperSection.Instance.DumpToLocal.Folder.TrimEnd('/').ToLowerInvariant();
            _excludePaths = BootstrapperSection.Instance.HttpModules.PathRewriter.IgnorePaths.Split(';');

            if (!_rewriteBasePath.StartsWith("~"))
                _rewriteBasePath = "~" + _rewriteBasePath;

            Trace.TraceInformation("[PathRewriterHttpModule]: Initialized with basepath: '{0}'", _rewriteBasePath);
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += (s, e) => ContextOnBeginRequest(context);
        }

        private static void ContextOnBeginRequest(HttpApplication application)
        {
            //check if there's a route matching current URL (MVC)
            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(application.Context));
            if (routeData != null)
            {
                Trace.TraceInformation("[PathRewriterHttpModule]: MatchedRoute: '{0}', Route: '{1}'", routeData.RouteHandler, routeData.Route);
                if (!(routeData.RouteHandler is StopRoutingHandler))
                    return;
            }

            string path = application.Request.Url.AbsolutePath.TrimEnd('/').ToLowerInvariant();

            //se o path não precisa ser reescrito
            if (path.StartsWith(_rewriteBasePath.TrimStart('~')))
                return;

            foreach (var excludePath in _excludePaths)
            {
                if (path.StartsWith(excludePath, StringComparison.OrdinalIgnoreCase))
                    return;
            }

            var isDirectory = string.IsNullOrEmpty(Path.GetExtension(application.Request.Url.AbsolutePath));
            if (isDirectory)
            {
                if (string.IsNullOrWhiteSpace(path) ||
                    !HostingEnvironment.VirtualPathProvider.DirectoryExists(application.Request.Url.AbsolutePath))
                {
                    CheckSegments(application.Context, application.Request.Url);
                }
                else
                {
                    RewritePath(application.Context, application.Request.Url, path);
                }
            }
            else
            {
                if (!HostingEnvironment.VirtualPathProvider.FileExists(application.Request.Url.AbsolutePath))
                {
                    CheckFullPath(application.Context, application.Request.Url);
                }
            }
        }

        public void Dispose()
        {
        }

        static void RewritePath(HttpContext context, Uri original, string virtualPath)
        {
            //var finalPath = virtualPath;
            //var pathInfo = original.Segments[original.Segments.Count() - 1];

            context.RewritePath(virtualPath, false);
            Trace.TraceInformation("[PathRewriterHttpModule]:Path Rewritten from '{0}' to '{1}'", original.AbsolutePath, virtualPath);
        }


        static void CheckFullPath(HttpContext context, Uri uri)
        {
            var virtualPath = string.Format("{0}/{1}", _rewriteBasePath, uri.AbsolutePath.Trim('/'));

            if (HostingEnvironment.VirtualPathProvider.FileExists(virtualPath))
            {
                RewritePath(context, uri, virtualPath);
            }
        }

        static void CheckSegments(HttpContext context, Uri uri)
        {
            //compatibilidade com webpages url routing

            string finalRewrite = string.Format("{0}/{1}", _rewriteBasePath, uri.AbsolutePath).Trim('/');

            const string defaultcshtml = "default.cshtml";
            var segs = uri.Segments;
            var count = segs.Count();

            var paths = new List<string>();
            for (var i = 0; i < count; i++)
            {
                var directory = string.Join("", segs, 0, count - i);
                paths.Add(VirtualPathUtility.AppendTrailingSlash(directory));
            }

            foreach (var path in paths)
            {
                //tentativa 1
                string virtualPath = string.Format("{0}/{1}/{2}", _rewriteBasePath, path.Trim('/'), defaultcshtml);
                if (HostingEnvironment.VirtualPathProvider.FileExists(virtualPath))
                {
                    RewritePath(context, uri, finalRewrite);
                    return;
                }
                //tentativa 2
                var newPath = path.TrimEnd('/') + ".cshtml";
                virtualPath = string.Format("{0}/{1}", _rewriteBasePath.TrimEnd('/'), newPath.Trim('/'));
                if (HostingEnvironment.VirtualPathProvider.FileExists(virtualPath))
                {
                    RewritePath(context, uri, finalRewrite);
                    return;
                }
            }
        }
    }
}