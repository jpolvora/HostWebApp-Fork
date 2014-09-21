using System.Web;
using System.Web.WebPages;
using System.Web.WebPages.Html;
using Frankstein.Common.Configuration;

namespace Frankstein.Common.Mvc
{
    public static class WebPagesHtmlHelpers
    {
        private static readonly string BasePath = BootstrapperSection.Instance.DumpToLocal.Folder;

        public static string MapPath(this WebPage page, string relativePath)
        {
            return MapPath(relativePath);
        }

        public static string MapPath(string relativePath)
        {
            var result = string.Format("{0}/{1}", BasePath.TrimEnd('/'), relativePath.TrimStart('~', '/'));
            return result;
        }
    }
}