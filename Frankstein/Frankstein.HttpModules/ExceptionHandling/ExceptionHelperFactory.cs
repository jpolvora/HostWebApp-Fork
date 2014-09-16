using System;
using System.Web;
using Frankstein.Common.Configuration;

namespace Frankstein.HttpModules.ExceptionHandling
{
    public static class ExceptionHelperFactory<TException> where TException : Exception
    {
        public static ExceptionHelper<TException> Create(HttpApplication application)
        {
            var cfg = BootstrapperSection.Instance.HttpModules;
            var path = BootstrapperSection.Instance.HttpModules.CustomError.ErrorViewPath;
            if (cfg.CustomError.UseRazor)
            {
                return new RazorRenderExceptionHelper<TException>(application, path);
            }

            return new ExceptionHelper<TException>(application, path);
        }
    }
}