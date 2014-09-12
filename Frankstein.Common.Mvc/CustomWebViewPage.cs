using System;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Frankstein.Common.Mvc
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
            using (DisposableTimer.StartNew("CustomWebViewPage: " + this.VirtualPath))
            {
                if (IsAjax || string.IsNullOrWhiteSpace(Layout))
                {
                    base.ExecutePageHierarchy();
                    return;
                }

                using (Output.BeginChunk("div", VirtualPath, false, "view"))
                {
                    base.ExecutePageHierarchy();
                }
            }
        }

        public HelperResult RenderSectionEx(string name, bool required = false)
        {
            var result = RenderSection(name, required);

            if (result == null)
                return null;

            var page = this;

            //encapsula o resultado da section num novo resultado
            return new HelperResult(writer =>
            {
                using (writer.BeginChunk("div", string.Format("{0}_{1}", page.VirtualPath, name), true, "section"))
                {
                    try
                    {
                        result.WriteTo(writer);
                    }
                    catch
                    {
                        //já foi renderizado
                    }
                }
            });
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
            using (DisposableTimer.StartNew("CustomWebViewPage<" + typeof(T).Name + ">: " + this.VirtualPath))
            {
                if (IsAjax || string.IsNullOrWhiteSpace(Layout))
                {
                    base.ExecutePageHierarchy();
                    return;
                }

                using (Output.BeginChunk("div", VirtualPath, false, "view"))
                {
                    base.ExecutePageHierarchy();
                }
            }
        }

        public HelperResult RenderSectionEx(string name, bool required = false)
        {
            var result = RenderSection(name, required);

            if (result == null)
                return null;

            var page = this;

            //encapsula o resultado da section num novo resultado
            return new HelperResult(writer =>
            {
                using (writer.BeginChunk("div", string.Format("{0}_{1}", page.VirtualPath, name), true, "section"))
                {
                    try
                    {
                        result.WriteTo(writer);
                    }
                    catch
                    {
                        //já foi renderizado
                    }
                }
            });
        }
    }
}