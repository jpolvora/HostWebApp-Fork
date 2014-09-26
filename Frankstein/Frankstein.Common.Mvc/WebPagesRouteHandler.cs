using System.Web;
using System.Web.Routing;
using System.Web.WebPages;

namespace Frankstein.Common.Mvc
{
    public class WebPagesRouteHandler : IRouteHandler
    {
        private readonly string _virtualPath;
        private Route _routeVirtualPath;

        public WebPagesRouteHandler(string virtualPath)
        {
            _virtualPath = virtualPath;
        }

        private Route RouteVirtualPath
        {
            get
            {
                if (_routeVirtualPath == null)
                {
                    _routeVirtualPath = new Route(_virtualPath.Substring(2), this);
                }
                return this._routeVirtualPath;
            }
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var substitutedVirtualPath = GetSubstitutedVirtualPath(requestContext);
            int index = substitutedVirtualPath.IndexOf('?');
            if (index != -1)
            {
                substitutedVirtualPath = substitutedVirtualPath.Substring(0, index);
            }
            requestContext.HttpContext.Items[HttpContextExtensions.RouteKey] = requestContext.RouteData.Values;
            return WebPageHttpHandler.CreateFromVirtualPath(substitutedVirtualPath);
        }

        public string GetSubstitutedVirtualPath(RequestContext requestContext)
        {
            VirtualPathData virtualPath = RouteVirtualPath.GetVirtualPath(requestContext, requestContext.RouteData.Values);
            if (virtualPath == null)
            {
                return _virtualPath;
            }
            return ("~/" + virtualPath.VirtualPath);
        }
    }
}