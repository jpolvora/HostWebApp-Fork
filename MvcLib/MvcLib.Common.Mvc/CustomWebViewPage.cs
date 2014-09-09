using System;
using System.Web.Mvc;
using System.Web.WebPages;

namespace MvcLib.Common.Mvc
{
    public class CustomWebViewPage : WebViewPage
    {
        public override void Execute()
        {

        }

        public bool IsRazorWebPage
        {
            get { return false; }
        }

        public override void ExecutePageHierarchy()
        {
            if (IsAjax)
            {
                base.ExecutePageHierarchy();
                return;
            }

            using (DisposableTimer.StartNew("CustomWebViewPage: " + this.VirtualPath))
            {
                using (this.BeginChunk("div", VirtualPath, "view"))
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

    public class CustomWebViewPage<T> : WebViewPage<T>
    {
        public override void Execute()
        {
            //actually this is never called
            throw new NotImplementedException();
        }

        public bool IsRazorWebPage
        {
            get { return false; }
        }

        public override void ExecutePageHierarchy()
        {
            if (IsAjax)
            {
                base.ExecutePageHierarchy();
                return;
            }

            using (DisposableTimer.StartNew("CustomWebViewPage<" + typeof(T).Name + ">: " + this.VirtualPath))
            {
                using (this.BeginChunk("div", VirtualPath, "view"))
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