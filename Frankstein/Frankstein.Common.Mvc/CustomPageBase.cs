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

        public bool IsRazorWebPage
        {
            get { return true; }
        }


        public override void ExecutePageHierarchy()
        {
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