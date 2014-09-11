using System;
using System.Diagnostics;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Razor;
using System.Web.Razor.Generator;
using System.Web.Routing;
using System.Web.WebPages;
using System.Web.WebPages.Razor;

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

            //WebPageRazorHost.AddGlobalImport("MyApp");
            //WebPageRazorHost.AddGlobalImport("MyApp.Models");
            //RazorBuildProvider.CodeGenerationCompleted += RazorBuildProviderOnCodeGenerationCompleted;
            //BuildProvider.RegisterBuildProvider(".csmd", typeof(RazorBuildProvider));
            //RazorCodeLanguage.Languages.Add("csmd", new CSharpRazorCodeLanguage());
            //WebPageHttpHandler.RegisterExtension("csmd");

        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

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