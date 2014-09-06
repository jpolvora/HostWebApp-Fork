﻿using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MvcLib.Common;
using MvcLib.Common.Configuration;

namespace HostWebApp
{
    public class Global : System.Web.HttpApplication
    {
        private static string _rewriteBasePath;

        protected void Application_Start(object sender, EventArgs e)
        {
            _rewriteBasePath = BootstrapperSection.Instance.DumpToLocal.Folder.TrimEnd('/');
            if (!_rewriteBasePath.StartsWith("~"))
                _rewriteBasePath = "~" + _rewriteBasePath;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Trace.TraceInformation("[GLOBAL]: Debugging Enabled: {0}", HttpContext.Current.IsDebuggingEnabled);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string path = Request.Url.AbsolutePath;

            if (path.StartsWith(_rewriteBasePath.Substring(1)))
                return;

            string virtualPath = string.Format("{0}{1}", _rewriteBasePath, path);

            if (string.IsNullOrWhiteSpace(VirtualPathUtility.GetFileName(virtualPath)))
            {
                return;
            }

            var physicalPath = Server.MapPath(virtualPath);

            if (!File.Exists(physicalPath)) return;

            string newpath = string.Format("{0}{1}", _rewriteBasePath.Substring(1), path);

            Trace.TraceInformation("Rewriting path from '{0}' to '{1}", path, newpath);
            Context.RewritePath(newpath);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}