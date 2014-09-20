using System;
using System.Web;
using Frankstein.Common;
using Frankstein.Common.Mvc.ExceptionHandling;

namespace Frankstein.HttpModules
{
    public class CustomErrorHttpModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.Error += OnError;
        }

        static void OnError(object sender, EventArgs args)
        {
            /*
                sample web.config entry
             */
            //<httpErrors errorMode="Custom" existingResponse="Auto" defaultResponseMode="ExecuteURL" defaultPath="/500.cshtml">
            //  <remove statusCode="403" />
            //  <error statusCode="403" path="/403.cshtml" responseMode="ExecuteURL" />
            //  <remove statusCode="404" />
            //  <error statusCode="404" path="/404.cshtml" responseMode="ExecuteURL" />
            //  <remove statusCode="500" />
            //  <error statusCode="500" path="/500.cshtml" responseMode="ExecuteURL" />
            //</httpErrors>

            var application = sender as HttpApplication;
            if (application == null)
                return;

            using (var helper = ExceptionHelperFactory<CustomException>.Create(application))
            {
                helper.HandleError();
            }
        }

        public void Dispose()
        {
        }
    }
}