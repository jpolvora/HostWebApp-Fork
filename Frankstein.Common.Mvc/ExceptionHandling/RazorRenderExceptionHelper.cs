using System;
using System.Diagnostics;
using System.Net.Mail;
using System.Threading;
using System.Web;
using Frankstein.Common.Configuration;

namespace Frankstein.Common.Mvc.ExceptionHandling
{
    public class RazorRenderExceptionHelper<TException> : ExceptionHelper<TException>
        where TException : Exception
    {
        public RazorRenderExceptionHelper(HttpApplication applicationInstance, string errorViewPath)
            : base(applicationInstance, errorViewPath, LogActionEx)
        {
        }

        static void LogActionEx(HttpException exception)
        {
            var status = exception.GetHttpCode();
            if (status < 500) return;
            var cfg = BootstrapperSection.Instance;

            if (cfg.Mail.SendExceptionToDeveloper &&
                (HttpContext.Current == null || !HttpContext.Current.IsDebuggingEnabled))
            {

                ThreadPool.QueueUserWorkItem(x => SendEmail(exception, cfg.AppName, new MailAddress(cfg.Mail.MailAdmin, "Admin"),
                    new MailAddress(cfg.Mail.MailDeveloper, "Developer")));
            }
            else
            {
                LogEvent.Raise(exception.Message, exception.GetBaseException());
            }
        }

        static void SendEmail(HttpException exception, string appName, MailAddress from, MailAddress to)
        {
            Trace.TraceInformation("[RazorRenderExceptionHelper]: Preparing to send email to developer");
            string body = exception.GetHtmlErrorMessage();
            bool html = true;
            if (string.IsNullOrWhiteSpace(body))
            {
                body = exception.ToString();
                html = false;
            }

            string subject = string.Format("Exception for app: {0}, at {1}", appName, DateTime.Now);

            EmailExtensions.SendEmail(from, to, subject, body, !html,
                (message, ex) =>
                {
                    if (ex == null)
                    {

                        Trace.TraceInformation("[RazorRenderExceptionHelper]: Email was sent to {0}",
                            to);
                    }
                    else
                    {
                        Trace.TraceError("[RazorRenderExceptionHelper]: Failed to send email. {0}", ex.Message);
                        LogEvent.Raise(exception.Message, exception.GetBaseException());
                    }
                });
        }

        protected override bool IsProduction()
        {
            //checa se o ambiente é de produção
            bool release = !Config.IsInDebugMode;

            return release;
        }

        protected override void RenderCustomException(TException exception)
        {
            //ApplicationInstance.Context.RewritePath(ErrorViewPath);

            Trace.TraceInformation("[RazorRenderExceptionHelper]: Rendering Custom Exception");
            var model = new ErrorModel()
            {
                Message = exception.Message,
                FullMessage = exception.ToString(),
                StackTrace = exception.StackTrace,
                Url = ApplicationInstance.Request.RawUrl,
                StatusCode = ApplicationInstance.Response.StatusCode,
            };

            Trace.TraceWarning("Rendering razor view: {0}", ErrorViewPath);
            var html = ViewRenderer.RenderView(ErrorViewPath, model);
            ApplicationInstance.Response.Write(html);

        }
    }

    public class ErrorModel
    {
        public string Message { get; set; }
        public string FullMessage { get; set; }
        public string StackTrace { get; set; }
        public string Url { get; set; }
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Message, Url);
        }
    }
}