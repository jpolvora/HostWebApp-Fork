using System;
using System.Web.WebPages;

namespace Frankstein.Common.Mvc
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

        private bool _stoped;
        private const string Stop = "_Stop";

        public bool IsRazorWebPage
        {
            get { return true; }
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
            Response.End();
        }

        /// <summary>
        /// Causes stop executing page hierarchy and redirect without throwing ThreadAbortException
        /// </summary>
        /// <param name="extraQuery"></param>
        public void StopAndRedirectSafeToDefaultUrl(string extraQuery = "")
        {
            if (_stoped)
                return;
            this.PageData[Stop] = true;
            this.Context.RedirectSafeToDefault(extraQuery);
            _stoped = true;
            Response.End();
        }

        public override void ExecutePageHierarchy()
        {
            if (_stoped || PageData.ContainsKey(Stop))
                return;

            using (DisposableTimer.StartNew("CustomPageBase: " + this.VirtualPath))
            {
                if (IsAjax || string.IsNullOrWhiteSpace(Layout))
                {
                    base.ExecutePageHierarchy();
                    return;
                }


                using (Output.BeginChunk("div", VirtualPath, false, "page"))
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
                        {
                            var @out = result.ToHtmlString();
                            writer.Write(@out);
                        }
                        else if (defHelperResult != null)
                        {
                            var def = defHelperResult(writer);
                            writer.Write(def);
                        }
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