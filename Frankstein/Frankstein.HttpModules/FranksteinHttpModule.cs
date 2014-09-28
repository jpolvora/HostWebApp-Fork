using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Frankstein.Common;
using Frankstein.Common.Configuration;
using Frankstein.Common.Mvc;
using Frankstein.Common.Mvc.Scheduling;

namespace Frankstein.HttpModules
{
    public class FranksteinHttpModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += BeginRequest;
        }

        private static void BeginRequest(object sender, EventArgs e)
        {
            var isFirstRequest = RequestCheck.IsFirstRequest();
            if (isFirstRequest)
            {
                Trace.TraceInformation("[FranksteinHttpModule]: First request arrived. ");

                //start job scheduler

                if (BootstrapperSection.Instance.Tasks.Enabled)
                {
                    var interval = BootstrapperSection.Instance.Tasks.Interval;
                    var job = new RecurringActionJob("Frankstein Recurring Task", interval, PingHost, true);
                    job.Register();
                }
            }
        }

        private async static void PingHost()
        {
            if (Config.IsInDebugMode)
                return;

            using (DisposableTimer.StartNew("PingHost Task ..."))
            {
                var url = string.Format("{0}?source={1}", RequestCheck.HostUrl, Guid.NewGuid().ToString("N"));
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

        public void Dispose()
        {

        }
    }
}