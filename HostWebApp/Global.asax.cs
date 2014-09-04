using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace HostWebApp
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
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

            string virtualPath = string.Format("~/dbfiles{0}", path);

            var physicalPath = Server.MapPath(virtualPath);

            if (File.Exists(physicalPath) || Directory.Exists(physicalPath))
            {
                string newpath = string.Format("/dbfiles{0}", path);

                Trace.TraceInformation("Rewriting path from '{0}' to '{1}", path, newpath);
                Context.RewritePath(newpath);
            }
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