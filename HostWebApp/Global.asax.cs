using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Frankstein.Common;
using Frankstein.Common.Configuration;
using Frankstein.Common.Mvc.Jobs;
using Frankstein.PluginLoader;
using HostWebApp.App_Start;

// ReSharper disable once CheckNamespace
namespace HostWebApp
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);


            PluginStorage.ExecutePlugins((s, exception) => exception.SendExceptionToDeveloper("Error executando plugin: " + s));

            var job = new ForeverActionJob("teste", 60, ExecuteJob);
            job.Start();

        }

        private static async System.Threading.Tasks.Task ExecuteJob()
        {
            using (DisposableTimer.StartNew("Scheduled Task ..."))
            {
                var webRequest = WebRequest.Create("http://hostwebapp.apphb.com?source=task");
                webRequest.Method = "HEAD";
                using (var webResponse = await webRequest.GetResponseAsync())
                {
                    string html = "";
                    var rStr = webResponse.GetResponseStream();
                    using (var sr = new StreamReader(rStr))
                    {
                        html = await sr.ReadToEndAsync();
                    }

                    await EmailExtensions.SendEmailAsync(new MailAddress(BootstrapperSection.Instance.Mail.MailAdmin),
                        new MailAddress(BootstrapperSection.Instance.Mail.MailDeveloper),
                        "Task Execution" + BootstrapperSection.Instance.AppName,
                        html, false, (message, exception) => { });

                    Trace.TraceInformation(((HttpWebResponse)webResponse).StatusCode.ToString());
                }
            }
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