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
            if (IsAjax || string.IsNullOrWhiteSpace(Layout))
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

        public override HelperResult RenderPage(string path, params object[] data)
        {
            using (this.BeginChunk("div", path, "page"))
            {
                return base.RenderPage(path, data);
            }
        }

        public HelperResult RenderSectionEx(string name, bool required = false)
        {
            using (this.BeginChunk("div", name, "section"))
            {
                return RenderSection(name, false);
            }
        }
    }
}