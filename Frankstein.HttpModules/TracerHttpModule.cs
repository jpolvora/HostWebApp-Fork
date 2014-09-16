using System;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frankstein.Common.Configuration;
using Frankstein.Common.Mvc;

namespace Frankstein.HttpModules
{
    public class TracerHttpModule : IHttpModule
    {
        private const string RequestId = "_request:id";

        private const string Stopwatch = "_request:sw";
        private static readonly string[] EventsToTrace = new string[0];
        private static readonly bool Verbose;

        public void Dispose()
        {
            StopTimer(HttpContext.Current.ApplicationInstance);
        }

        static TracerHttpModule()
        {
            EventsToTrace = BootstrapperSection.Instance.HttpModules.Trace.Events.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Verbose = BootstrapperSection.Instance.HttpModules.Trace.Verbose;
        }

        public void Init(HttpApplication on)
        {
            //http://msdn.microsoft.com/en-us/library/bb470252(v=vs.100).aspx

            on.Error += (sender, args) => OnError(sender);
            on.BeginRequest += (sender, args) => OnBeginRequest(on);
            on.EndRequest += (sender, args) => OnEndRequest(on);

            if (EventsToTrace.Contains("AuthenticateRequest") || EventsToTrace.Length == 0)
            {
                Trace.TraceInformation("Subscribing to 'AuthenticateRequest' event");
                on.AuthenticateRequest += (sender, args) => TraceNotification(on, "AuthenticateRequest");
            }

            if (EventsToTrace.Contains("PostAuthenticateRequest") || EventsToTrace.Length == 0)
            {
                Trace.TraceInformation("Subscribing to 'PostAuthenticateRequest' event");
                on.PostAuthenticateRequest += (sender, args) => TraceNotification(on, "PostAuthenticateRequest");
            }

            if (EventsToTrace.Contains("AuthorizeRequest") || EventsToTrace.Length == 0)
            {
                Trace.TraceInformation("Subscribing to 'AuthorizeRequest' event");
                on.AuthorizeRequest += (sender, args) => TraceNotification(on, "AuthorizeRequest");
            }

            if (EventsToTrace.Contains("PostAuthorizeRequest") || EventsToTrace.Length == 0)
            {
                Trace.TraceInformation("Subscribing to 'PostAuthorizeRequest' event");
                on.PostAuthorizeRequest += (sender, args) => TraceNotification(on, "PostAuthorizeRequest");
            }

            if (EventsToTrace.Contains("ResolveRequestCache") || EventsToTrace.Length == 0)
            {
                Trace.TraceInformation("Subscribing to 'ResolveRequestCache' event");
                on.ResolveRequestCache += (sender, args) => TraceNotification(on, "ResolveRequestCache");
            }

            if (EventsToTrace.Contains("PostResolveRequestCache") || EventsToTrace.Length == 0)
            {
                Trace.TraceInformation("Subscribing to 'PostResolveRequestCache' event");
                //MVC Routing module remaps the handler here.
                on.PostResolveRequestCache += (sender, args) => TraceNotification(on, "PostResolveRequestCache");
            }

            if (EventsToTrace.Contains("MapRequestHandler") || EventsToTrace.Length == 0)
            {
                Trace.TraceInformation("Subscribing to 'MapRequestHandler' event");
                //only iis7
                on.MapRequestHandler += (sender, args) => TraceNotification(on, "MapRequestHandler");
            }

            if (EventsToTrace.Contains("PostMapRequestHandler") || EventsToTrace.Length == 0)
            {
                Trace.TraceInformation("Subscribing to 'PostMapRequestHandler' event");

                //An appropriate handler is selected based on the file-name extension of the requested resource. 
                //The handler can be a native-code module such as the IIS 7.0 StaticFileModule or a managed-code module
                //such as the PageHandlerFactory class (which handles .aspx files). 
                on.PostMapRequestHandler += (sender, args) => TraceNotification(on, "PostMapRequestHandler");
            }

            if (EventsToTrace.Contains("AcquireRequestState") || EventsToTrace.Length == 0)
            {
                Trace.TraceInformation("Subscribing to 'AcquireRequestState' event");
                on.AcquireRequestState += (sender, args) => TraceNotification(on, "AcquireRequestState");
            }

            if (EventsToTrace.Contains("PostAcquireRequestState") || EventsToTrace.Length == 0)
            {
                Trace.TraceInformation("Subscribing to 'PostAcquireRequestState' event");
                on.PostAcquireRequestState += (sender, args) => TraceNotification(on, "PostAcquireRequestState");
            }

            if (EventsToTrace.Contains("PreRequestHandlerExecute") || EventsToTrace.Length == 0)
            {
                Trace.TraceInformation("Subscribing to 'PreRequestHandlerExecute' event");
                on.PreRequestHandlerExecute += (sender, args) => TraceNotification(on, "PreRequestHandlerExecute");
            }

            if (EventsToTrace.Contains("PostRequestHandlerExecute") || EventsToTrace.Length == 0)
            {
                Trace.TraceInformation("Subscribing to 'PostRequestHandlerExecute' event");
                //after Call the ProcessRequest of IHttpHandler
                on.PostRequestHandlerExecute += (sender, args) => TraceNotification(on, "PostRequestHandlerExecute");
            }

            if (EventsToTrace.Contains("ReleaseRequestState") || EventsToTrace.Length == 0)
            {
                Trace.TraceInformation("Subscribing to 'ReleaseRequestState' event");
                on.ReleaseRequestState += (sender, args) => TraceNotification(on, "ReleaseRequestState");
            }

            if (EventsToTrace.Contains("PostReleaseRequestState") || EventsToTrace.Length == 0)
            {
                Trace.TraceInformation("Subscribing to 'PostReleaseRequestState' event");
                //Perform response filtering if the Filter property is defined.
                on.PostReleaseRequestState += (sender, args) => TraceNotification(on, "PostReleaseRequestState");
            }

            if (EventsToTrace.Contains("UpdateRequestCache") || EventsToTrace.Length == 0)
            {
                Trace.TraceInformation("Subscribing to 'UpdateRequestCache' event");
                on.UpdateRequestCache += (sender, args) => TraceNotification(on, "UpdateRequestCache");
            }

            if (EventsToTrace.Contains("PostUpdateRequestCache") || EventsToTrace.Length == 0)
            {
                Trace.TraceInformation("Subscribing to 'PostUpdateRequestCache' event");
                on.PostUpdateRequestCache += (sender, args) => TraceNotification(on, "PostUpdateRequestCache");
            }

            if (EventsToTrace.Contains("LogRequest") || EventsToTrace.Length == 0)
            {
                Trace.TraceInformation("Subscribing to 'LogRequest' event");
                //The MapRequestHandler, LogRequest, and PostLogRequest events are supported only if the application 
                //is running in Integrated mode in IIS 7.0 and with the .NET Framework 3.0 or later.
                on.LogRequest += (sender, args) => TraceNotification(on, "LogRequest"); //iis7
            }

            if (EventsToTrace.Contains("PostLogRequest") || EventsToTrace.Length == 0)
            {
                Trace.TraceInformation("Subscribing to 'PostLogRequest' event");
                on.PostLogRequest += (sender, args) => TraceNotification(on, "PostLogRequest"); //iis7
            }

            if (EventsToTrace.Contains("PreSendRequestHeaders") || EventsToTrace.Length == 0)
            {
                Trace.TraceInformation("Subscribing to 'PreSendRequestHeaders' event");
                on.PreSendRequestHeaders += (sender, args) => TraceNotification(on, "PreSendRequestHeaders");
            }

            if (EventsToTrace.Contains("PreSendRequestContent") || EventsToTrace.Length == 0)
            {
                Trace.TraceInformation("Subscribing to 'PreSendRequestContent' event");
                on.PreSendRequestContent += (sender, args) => TraceNotification(on, "PreSendRequestContent");
            }
        }

        private static void OnBeginRequest(HttpApplication application)
        {
            var context = application.Context;

            var rid = new Random().Next(1, 99999).ToString("d5");
            context.Items.Add(RequestId, rid);

            context.Items[Stopwatch] = System.Diagnostics.Stopwatch.StartNew();

            bool isAjax = context.Request.IsAjaxRequest();

            if (isAjax)
            {
                context.Response.SuppressFormsAuthenticationRedirect = true;
            }

            if (Verbose)
            {
                if (context.Items.Contains("IIS_WasUrlRewritten") || context.Items.Contains("HTTP_X_ORIGINAL_URL"))
                {
                    Trace.TraceInformation("[TracerHttpModule]:Url was rewriten from '{0}' to '{1}'",
                        context.Request.ServerVariables["HTTP_X_ORIGINAL_URL"],
                        context.Request.ServerVariables["SCRIPT_NAME"]);
                }

                Trace.TraceInformation("[BeginRequest]:[{0}] {1} {2} {3} [{4}]", rid, context.Request.HttpMethod,
                    context.Request.RawUrl, isAjax ? "Ajax: True" : "", context.Request.UrlReferrer);
            }
        }

        private static void TraceNotification(HttpApplication application, string eventName)
        {
            var context = application.Context;
            var isPost = context.IsPostNotification;
            var postString = isPost ? "[POST]" : "[PRE]";

            var rid = application.Context.Items[RequestId];
            Trace.TraceInformation("[TracerHttpModule]:{0}, {1}, rid: [{2}], [{3}], {4} {5}",
                postString, eventName, rid, application.Context.CurrentHandler,
                application.User != null ? application.User.Identity.Name : "-", application.Context.Response.StatusCode);


            switch (context.CurrentNotification)
            {
                case RequestNotification.PreExecuteRequestHandler:
                    {
                        //will call ProcessRequest of IHttpHandler

                        var mvcHandler = context.Handler as MvcHandler;
                        if (mvcHandler != null)
                        {
                            var controller = mvcHandler.RequestContext.RouteData.GetRequiredString("controller");
                            var action = mvcHandler.RequestContext.RouteData.GetRequiredString("action");
                            var area = mvcHandler.RequestContext.RouteData.DataTokens["area"];

                            Trace.TraceInformation(
                                "Entering MVC Pipeline. Area: '{0}', Controller: '{1}', Action: '{2}'", area,
                                controller, action);
                        }
                        else
                        {
                            Trace.TraceInformation("[TracerHttpModule]:Executing ProcessRequest of Handler {0}", context.CurrentHandler);
                        }
                    }
                    break;
                case RequestNotification.ReleaseRequestState:
                    {
                        Trace.TraceInformation("[TracerHttpModule]:Response Filter ({0}): {1}", postString, context.Response.Filter);
                        break;
                    }
            }
        }

        private static void OnEndRequest(HttpApplication application)
        {
            StopTimer(application);
            Trace.Flush();

            if (!Verbose)
                return;

            var context = application.Context;

            var rid = context.Items[RequestId];

            var msg = string.Format("[EndRequest]:[{0}], Content-Type: {1}, Status: {2}, Render: {3}, url: {4}",
                rid, context.Response.ContentType, context.Response.StatusCode, GetTime(application),
                context.Request.Url);
            Trace.TraceInformation(msg);

            if (context.Request.IsAuthenticated && context.Response.StatusCode == 403)
            {
                bool isAjax = context.Request.IsAjaxRequest();
                if (!isAjax)
                {
                    context.Response.Write("Você está autenticado mas não possui permissões para acessar este recurso");
                }
            }
        }

        private static void OnError(object sender)
        {
            var application = (HttpApplication)sender;
            StopTimer(application);
            Trace.Flush();

            if (!Verbose)
                return;

            var rid = application.Context.Items[RequestId];
            Trace.TraceInformation("[TracerHttpModule]: Error at {0}, request {1}, Handler: {2}, Message:'{3}'",
                application.Context.CurrentNotification, rid, application.Context.CurrentHandler,
                application.Context.Error);
        }

        private static void StopTimer(HttpApplication application)
        {
            if (application == null || application.Context == null) return;
            var stopwatch = application.Context.Items[Stopwatch] as Stopwatch;
            if (stopwatch != null)
                stopwatch.Stop();
        }

        private static double GetTime(HttpApplication application)
        {
            if (application == null || application.Context == null) return -1;

            var stopwatch = application.Context.Items[Stopwatch] as Stopwatch;
            if (stopwatch != null)
            {
                var ts = stopwatch.Elapsed.TotalSeconds;
                return ts;
            }
            return -1;
        }
    }
}