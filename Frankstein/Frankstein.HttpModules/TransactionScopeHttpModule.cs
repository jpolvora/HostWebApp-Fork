using System;
using System.Diagnostics;
using System.Transactions;
using System.Web;
using Frankstein.Common;
using Frankstein.Common.Configuration;
using Frankstein.Common.Mvc;

namespace Frankstein.HttpModules
{
    /// <summary>
    /// it's not possible to ensure that transactions will start and dispose during a request
    /// because events on asp.net pipeline are not always executed on same thread,
    /// and transaction scopes are dependent on thread.
    /// So it only works if you get lucky of entering and exiting the request by the same thread.
    /// obs: We can use TransactionScopes this way by creating a "Custom Pipeline" on the ProcessRequest of an IHttpHandler.
    /// </summary>
    public class TransactionScopeHttpModule : IHttpModule
    {
        private const string ScopeKey = "_TransScope_";

        public void Init(HttpApplication context)
        {
            //changed from BeginRequest to AuthenticateRequest in order to avoid TransactionTimeOut
            context.AuthenticateRequest += ContextOnAuthenticateRequest;
            context.Error += ContextOnError;
            context.EndRequest += ContextOnEndRequest;
        }

        private static void ContextOnAuthenticateRequest(object sender, EventArgs eventArgs)
        {
            var app = (HttpApplication)sender;
            var context = app.Context;
            var timeout = TimeSpan.FromSeconds(BootstrapperSection.Instance.HttpModules.TransactionScope.TimeOut);
            if (context.IsDebuggingEnabled)
                timeout = TimeSpan.Zero;

            var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions()
            {
                Timeout = timeout
            });

            context.Items[ScopeKey] = scope;
            var requestId = context.GetRequestId();
            Trace.TraceInformation("[TransactionScopeHttpModule]: Transaction created for Request [{0}]: {1}, timeout: {2}", requestId, scope, timeout);
        }

        private static void ContextOnError(object sender, EventArgs eventArgs)
        {
            var app = (HttpApplication)sender;
            var scope = GetTransactionScope(app.Context);

            if (scope != null)
            {
                scope.Dispose();
            }
        }

        private static void ContextOnEndRequest(object sender, EventArgs eventArgs)
        {
            var app = (HttpApplication)sender;
            var context = app.Context;

            if (context.Error != null) return; //rollback on error

            //completes the current request scope if there's not errors pending
            var scope = GetTransactionScope(context);

            var requestId = context.GetRequestId();
            if (scope != null)
            {
                using (DisposableTimer.StartNew("[TransactionScopeHttpModule]: Commiting Transaction for request [{0}]".Fmt(requestId)))
                {
                    scope.Complete();
                }
            }
            else
            {
                Trace.TraceWarning("[TransactionScopeHttpModule]: No TransactionScope found for Current Request: [{0}]. Error ?", requestId);
            }
        }

        internal static TransactionScope GetTransactionScope(HttpContext context)
        {
            if (context == null)
                return null;

            var scope = context.Items[ScopeKey] as TransactionScope;

            return scope;
        }

        internal static TransactionScope GetTransactionScope(HttpContextBase context)
        {
            if (context == null)
                return null;

            var scope = context.Items[ScopeKey] as TransactionScope;

            return scope;
        }

        public void Dispose()
        {
        }
    }
}