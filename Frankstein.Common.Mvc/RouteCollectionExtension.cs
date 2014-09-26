using System.Web.Routing;

namespace Frankstein.Common.Mvc
{
    public static class RouteCollectionExtension
    {
        public static void MapWebPageRoute(this RouteCollection routeCollection, string routeUrl, string virtualPath, object defaultValues = null, object constraints = null, string routeName = null)
        {
            routeName = routeName ?? routeUrl;

            Route item = new Route(routeUrl, new RouteValueDictionary(defaultValues), new RouteValueDictionary(constraints), new WebPagesRouteHandler(virtualPath));
            routeCollection.Add(routeName, item);
        }
    }
}