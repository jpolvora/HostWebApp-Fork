using System;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Web.WebPages;
using MvcLib.Common;
using MvcLib.Common.Configuration;
using MvcLib.Common.Mvc;

namespace MvcLib.HttpModules
{
    public class CustomErrorHttpModule : IHttpModule
    {
        private string _errorViewPath;

        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
            context.Error += OnError;

            _errorViewPath = BootstrapperSection.Instance.HttpModules.CustomError.ErrorViewPath;
        }

        static void OnBeginRequest(object sender, EventArgs eventArgs)
        {
        }

        void OnError(object sender, EventArgs args)
        {
            //<httpErrors errorMode="Custom" existingResponse="Auto" defaultResponseMode="ExecuteURL">
            //  <remove statusCode="404"/>
            //  <error statusCode="404" path="/404.cshtml" responseMode="ExecuteURL"/>
            //  <remove statusCode="500"/>
            //  <error statusCode="500" path="/500.cshtml" responseMode="ExecuteURL"/>
            //</httpErrors>

            var application = (HttpApplication)sender;

            var context = application.Context;
            var server = application.Server;
            var response = application.Response;
            
            Exception ex = server.GetLastError();

            HttpException httpException = ex as HttpException ?? new HttpException("Unknown exception...", ex);

            var rootException = httpException.GetBaseException();

            Trace.TraceError("[CustomError]:Ocorreu um Erro: {0}", rootException.ToString());

            //checa se o ambiente é de produção
            bool release = ConfigurationManager.AppSettings["Environment"]
                .Equals("Release", StringComparison.OrdinalIgnoreCase);

            if (release)
            {
                //log or send email to developer notifiying the exception ?
                LogError(httpException);
                server.ClearError();
            }

            var statusCode = httpException.GetHttpCode();

            //setar o statuscode para que o IIS selecione a view correta (no web.config)
            response.StatusCode = statusCode;
            response.StatusDescription = rootException.Message; //todo: colocar uma msg melhor

            switch (statusCode)
            {
                case 404:
                    break;
                case 500:
                    {
                        //check for exception types you want to show custom info
                        //for example, business rules exceptions
                        if (!(rootException is CustomException))
                        {
                            //will show default 500.
                            //to show default 500 in dev mode, call Server.ClearError() 
                            //or modify web.config to debug=false
                            break;
                        }
                        server.ClearError();
                        response.TrySkipIisCustomErrors = true;
                        response.Clear();

                        try
                        {
                            //atualiza os paths para o request (exceto RawUrl)
                            context.RewritePath(_errorViewPath);

                            var model = new ErrorModel()
                            {
                                Message = rootException.Message,
                                FullMessage = rootException.ToString(),
                                StackTrace = rootException.StackTrace,
                                Url = application.Request.RawUrl,
                                StatusCode = statusCode,
                                StatusDescription = rootException.Message
                            };

                            RenderView(_errorViewPath, model, context);
                        }
                        catch
                        {
                            //erro ao renderizar a página customizada, então Response.Write como fallback
                            response.Write(rootException.ToString());
                        }
                        break;
                    }
            }
        }

        static void LogError(HttpException exception)
        {
            var status = exception.GetHttpCode();
            if (status >= 500)
                LogEvent.Raise(exception.Message, exception.GetBaseException());
        }

        private static void RenderView(string errorViewPath, ErrorModel model, HttpContext context)
        {
            var handler = WebPageHttpHandler.CreateFromVirtualPath(errorViewPath);
            context.Session["exception"] = model;
            handler.ProcessRequest(context);
            context.Session.Remove("exception");

            //var html = ViewRenderer.RenderView(errorViewPath, model);
            //response.Write(html);
        }

        public void Dispose()
        {
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
}