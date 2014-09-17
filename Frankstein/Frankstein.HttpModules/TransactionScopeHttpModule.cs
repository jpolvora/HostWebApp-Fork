using System;
using System.Diagnostics;
using System.Transactions;
using System.Web;
using Frankstein.Common.Configuration;
using Frankstein.Common.Mvc;

namespace Frankstein.HttpModules
{
    public class TransactionScopeHttpModule : IHttpModule
    {
        private const string ScopeKey = "_TransScope_";

        public void Init(HttpApplication context)
        {
            context.BeginRequest += ContextOnBeginRequest;
            context.Error += ContextOnError;
            context.EndRequest += ContextOnEndRequest;
        }

        private static void ContextOnBeginRequest(object sender, EventArgs eventArgs)
        {
            var app = (HttpApplication)sender;
            var context = app.Context;
            var timeout = TimeSpan.FromSeconds(BootstrapperSection.Instance.HttpModules.TransactionScope.TimeOut);
            if (context.IsDebuggingEnabled)
                timeout = TimeSpan.MaxValue;

            var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions()
            {
                Timeout = timeout
            });
            context.Items[ScopeKey] = scope;
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
                Trace.TraceInformation("Commiting transaction for request {0}", requestId);
                scope.Complete();
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