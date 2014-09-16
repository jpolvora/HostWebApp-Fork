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

        private bool _stoped;
        private const string Stop = "_Stop";

        public bool IsRazorWebPage
        {
            get { return false; }
        }

        /// <summary>
        /// Causes stop executing page hierarchy and redirect without throwing ThreadAbortException
        /// </summary>
        /// <param name="url"></param>
        public void StopAndRedirectSafe(string url)
        {
            if (_stoped)
                return;
            this.PageData[Stop] = true;
            this.Context.RedirectSafe(url);
            _stoped = true;
        }

        /// <summary>
        /// Causes stop executing page hierarchy and redirect without throwing ThreadAbortException
        /// </summary>
        /// <param name="extraQuery"></param>
        public void StopAndRedirectSafeToDefaultUrl(string extraQuery)
        {
            if (_stoped)
                return;
            this.PageData[Stop] = true;
            this.Context.RedirectSafeToDefault(extraQuery);
            _stoped = true;
        }

        public override void ExecutePageHierarchy()
        {
            if (_stoped || PageData.ContainsKey(Stop))
                return;

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

        public HelperResult RenderSectionEx(string name, bool required = false, Func<dynamic, HelperResult> defHelperResult = null)
        {
            if (_stoped || PageData.ContainsKey(Stop))
                return null;

            //encapsula o resultado da section num novo resultado
            return new HelperResult(writer =>
            {
                using (writer.BeginChunk("div", string.Format("{0}_{1}", this.VirtualPath, name), true, "section"))
                {
                    try
                    {
                        var result = RenderSection(name, required);
                        if (result != null)
                            writer.Write(result.ToHtmlString());
                        else if (defHelperResult != null)
                            writer.Write(defHelperResult(writer));
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

        private bool _stoped;
        private const string Stop = "_Stop";

        public bool IsRazorWebPage
        {
            get { return false; }
        }

        public void StopAndRedirectSafe(string url)
        {
            if (_stoped)
                return;
            this.PageData[Stop] = true;
            this.Context.RedirectSafe(url);
            _stoped = true;
        }

        public void StopAndRedirectSafeToDefaultUrl(string extraQuery)
        {
            if (_stoped)
                return;
            this.PageData[Stop] = true;
            this.Context.RedirectSafeToDefault(extraQuery);
            _stoped = true;
        }

        public override void ExecutePageHierarchy()
        {
            if (_stoped || PageData.ContainsKey(Stop))
                return;

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

        public HelperResult RenderSectionEx(string name, bool required = false, Func<dynamic, HelperResult> defHelperResult = null)
        {
            if (_stoped || PageData.ContainsKey("_Stop"))
                return null;

            //encapsula o resultado da section num novo resultado
            return new HelperResult(writer =>
            {
                using (writer.BeginChunk("div", string.Format("{0}_{1}", this.VirtualPath, name), true, "section"))
                {
                    try
                    {
                        var result = RenderSection(name, required);
                        if (result != null)
                            writer.Write(result.ToHtmlString());
                        else if (defHelperResult != null)
                            writer.Write(defHelperResult(writer));
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