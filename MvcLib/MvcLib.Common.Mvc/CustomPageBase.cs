using System;
using System.Web.WebPages;

namespace MvcLib.Common.Mvc
{
    /// <summary>
    /// Para WebPages
    /// </summary>
    public class CustomPageBase : WebPage
    {
        public override void Execute()
        {
            //actually this is never called
            throw new NotImplementedException();
        }

        public bool IsRazorWebPage
        {
            get { return true; }
        }


        public override void ExecutePageHierarchy()
        {
            if (IsAjax)
            {
                base.ExecutePageHierarchy();
                return;
            }

            using (DisposableTimer.StartNew("CustomPageBase: " + this.VirtualPath))
            {
                using (this.BeginChunk("div", VirtualPath, "page"))
                {
                    base.ExecutePageHierarchy();
                }
            }
        }

        public HelperResult RenderSectionEx(string name, bool required = false)
        {
            if (IsAjax)
            {
                return RenderSection(name, required);
            }

            using (this.BeginChunk("div", name, "section"))
            {
                return RenderSection(name, required);
            }
        }
    }
}