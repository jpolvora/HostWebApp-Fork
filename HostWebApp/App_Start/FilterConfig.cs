using System.Web;
using System.Web.Mvc;

namespace HostWebApp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //if (!HttpContext.Current.IsDebuggingEnabled)
            //    filters.Add(new HandleErrorAttribute());
        }
    }
}
