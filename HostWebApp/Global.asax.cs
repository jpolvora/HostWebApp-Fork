using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Frankstein.Common;
using Frankstein.Common.Configuration;
using Frankstein.Common.Mvc;
using Frankstein.Common.Mvc.Scheduling;
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
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (!FirstRequest.IsFirstRequest()) return;

            var job = new ForeverActionJob("test-task", 120, ExecuteJob);
            job.Register();
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

        private static async System.Threading.Tasks.Task ExecuteJob()
        {
            using (DisposableTimer.StartNew("Scheduled Task ..."))
            {
                var url = string.Format("{0}?source={1}", FirstRequest.HostUrl, Guid.NewGuid().ToString("N"));
                Trace.TraceInformation("Making a request to {0}", url);
                var webRequest = WebRequest.Create(url);
                webRequest.Method = "HEAD";
                using (var webResponse = await webRequest.GetResponseAsync())
                {
                    var httpResponse = (HttpWebResponse)webResponse;
                    Trace.TraceInformation("[Task]: Status = {0}", httpResponse.StatusCode);
                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
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

                        Trace.TraceInformation("Send task info email");

                        await
                            EmailExtensions.SendEmailAsync(new MailAddress(BootstrapperSection.Instance.Mail.MailAdmin),
                                new MailAddress(BootstrapperSection.Instance.Mail.MailDeveloper),
                                "Task Execution: " + BootstrapperSection.Instance.AppName,
                                html, false, (message, exception) => { });
                    }
                }
            }
        }
    }

    public class FirstRequest
    {
        private static readonly object Lock = new object();
        private static bool _wasInit;
        public static string HostUrl { get; private set; }

        public static bool IsFirstRequest()
        {
            if (_wasInit)
                return false;

            lock (Lock)
            {
                if (_wasInit)
                    return false;

                var context = HttpContext.Current;

                if (context == null || context.Request == null)
                    throw new Exception("HttpContext not available at this moment.");

                HostUrl = context.ToPublicUrl(new Uri("/", UriKind.Relative));

                _wasInit = true;
                return true;
            }
        }
    }
}