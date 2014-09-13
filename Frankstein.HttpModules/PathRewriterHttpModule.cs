using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Frankstein.Common.Configuration;

namespace Frankstein.HttpModules
{
    public class PathRewriterHttpModule : IHttpModule
    {
        private static readonly string _rewriteBasePath;

        static PathRewriterHttpModule()
        {
            _rewriteBasePath = BootstrapperSection.Instance.DumpToLocal.Folder.TrimEnd('/');
            if (!_rewriteBasePath.StartsWith("~"))
                _rewriteBasePath = "~" + _rewriteBasePath;

            Trace.TraceInformation("[PathRewriterHttpModule]: '{0}'", _rewriteBasePath);
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += (s, e) => ContextOnBeginRequest(context);
        }

        private static void ContextOnBeginRequest(HttpApplication application)
        {
            string path = application.Request.Url.AbsolutePath.TrimEnd('/');

            //se o path não precisa ser reescrito
            if (path.StartsWith(_rewriteBasePath.Substring(1)))
                return;

            var isDirectory = string.IsNullOrEmpty(Path.GetExtension(application.Request.Url.AbsolutePath));
            if (isDirectory)
            {
                if (CheckSegments(application.Context, application.Request.Url))
                    return;
            }
            else
            {
                if (CheckFullPath(application.Context, application.Request.Url))
                    return;
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
            Trace.TraceInformation("[PathRewriterHttpModule]:Rewriting path from '{0}' to '{1}'", original.AbsolutePath, virtualPath);
        }


        static bool CheckFullPath(HttpContext context, Uri uri)
        {
            var virtualPath = string.Format("{0}/{1}", _rewriteBasePath, uri.AbsolutePath.Trim('/'));

            if (HostingEnvironment.VirtualPathProvider.FileExists(virtualPath))
            {
                RewritePath(context, uri, virtualPath);
                return true;
            }
            return false;
        }

        static bool CheckSegments(HttpContext context, Uri uri)
        {
            //compatibilidade com webpages url routing

            string finalRewrite = string.Format("{0}/{1}", _rewriteBasePath.TrimEnd('/'), uri.AbsolutePath).Trim('/');

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
                string virtualPath = string.Format("{0}/{1}/{2}", _rewriteBasePath.TrimEnd('/'), path.Trim('/'), defaultcshtml);
                if (HostingEnvironment.VirtualPathProvider.FileExists(virtualPath))
                {
                    RewritePath(context, uri, finalRewrite);
                    return true;
                }
                //tentativa 2
                var newPath = path.TrimEnd('/') + ".cshtml";
                virtualPath = string.Format("{0}/{1}", _rewriteBasePath.TrimEnd('/'), newPath.Trim('/'));
                if (HostingEnvironment.VirtualPathProvider.FileExists(virtualPath))
                {
                    RewritePath(context, uri, finalRewrite);
                    return true;
                }
            }


            return false;
        }
    }
}