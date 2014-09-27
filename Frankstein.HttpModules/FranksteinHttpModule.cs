using System;
using System.Diagnostics;
using System.Web;
using Frankstein.Common.Mvc;

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
                Trace.TraceInformation("[FranksteinHttpModule]: First request arrived. ");
        }

        public void Dispose()
        {

        }
    }
}