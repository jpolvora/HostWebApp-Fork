using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
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
                    var httpResponse = (HttpWebResponse) webResponse;
                    
                    var headers = httpResponse.Headers;
                    var sb = new StringBuilder();
                    sb.AppendFormat("Status: {0}", httpResponse.StatusCode).AppendLine();
                    sb.AppendFormat("Description: {0}", httpResponse.StatusDescription).AppendLine();
                    sb.AppendFormat("Server: {0}", httpResponse.Server).AppendLine();
                    sb.AppendFormat("ContentLength: {0}", httpResponse.ContentLength).AppendLine();
                    sb.AppendFormat("ContentType: {0}", httpResponse.ContentType).AppendLine();
                    string html = sb.ToString();

                    foreach (var header in headers)
                    {
                        html += header + Environment.NewLine;
                    }
                  
                    await EmailExtensions.SendEmailAsync(new MailAddress(BootstrapperSection.Instance.Mail.MailAdmin),
                        new MailAddress(BootstrapperSection.Instance.Mail.MailDeveloper),
                        "Task Execution" + BootstrapperSection.Instance.AppName,
                        html, false, (message, exception) => { });

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