using System.Web;
using System.Web.Helpers;
using System.Web.WebPages;
using MarkdownDeep;

namespace Frankstein.Common.Mvc.MD
{
    /// <summary>
    /// Helper class for transforming Markdown.
    /// </summary>
    public static class MarkdownHelper
    {
        /// <summary>
        /// An instance of the Markdown class that performs the transformations.
        /// </summary>
        static readonly Markdown MarkdownTransformer = new Markdown();

        /// <summary>
        /// Transforms a string of Markdown into HTML.
        /// </summary>
        /// <param name="helper">HtmlHelper - Not used, but required to make this an extension method.</param>
        /// <param name="text">The Markdown that should be transformed.</param>
        /// <returns>The HTML representation of the supplied Markdown.</returns>
        public static IHtmlString Markdown(this System.Web.Mvc.HtmlHelper helper, string text)
        {
            // Transform the supplied text (Markdown) into HTML.
            string html = MarkdownTransformer.Transform(text);

            // Wrap the html in an MvcHtmlString otherwise it'll be HtmlEncoded and displayed to the user as HTML :(
            return new System.Web.Mvc.MvcHtmlString(html);
        }

        public static IHtmlString Markdown(this System.Web.WebPages.Html.HtmlHelper helper, string text)
        {
            var hash = text.GetHashCode().ToString();

            var cachedEntry = WebCache.Get(hash) as HtmlString;
            if (cachedEntry != null)
            {
                return cachedEntry;
            }

            // Transform the supplied text (Markdown) into HTML.
            string html = MarkdownTransformer.Transform(text);

            // Wrap the html in an MvcHtmlString otherwise it'll be HtmlEncoded and displayed to the user as HTML :(
            var result = new HtmlString(html);
            WebCache.Set(hash, result);

            return result;
        }
    }
}
