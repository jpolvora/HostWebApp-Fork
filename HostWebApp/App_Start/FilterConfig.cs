using System.Web.Mvc;

// ReSharper disable once CheckNamespace
namespace HostWebApp.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            /*part of the code for HandlerErrorAttribute was used in Frankstein/ExceptionHandler class.
             * so currently we don't need this attribute. */

            //filters.Add(new HandleErrorAttribute());
        }
    }
}
