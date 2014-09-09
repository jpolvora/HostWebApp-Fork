using System;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.WebPages;

namespace MvcLib.Common.Mvc
{
    public static class WebPageExtensions
    {
        public static string FingerPrint(string rootRelativePath)
        {
            if (HttpContext.Current.Request.IsLocal)
                return rootRelativePath;

            if (HttpRuntime.Cache[rootRelativePath] == null)
            {
                string relative = VirtualPathUtility.ToAbsolute("~" + rootRelativePath);
                string absolute = HostingEnvironment.MapPath(relative);

                if (!File.Exists(absolute))
                    throw new FileNotFoundException("File not found", absolute);

                DateTime date = File.GetLastWriteTime(absolute);
                int index = relative.LastIndexOf('.');

                string result = relative.Insert(index, "_" + date.Ticks);

                HttpRuntime.Cache.Insert(rootRelativePath, result, new CacheDependency(absolute));
            }

            return HttpRuntime.Cache[rootRelativePath] as string;
        }

        public static Chunk BeginChunk(this WebPageBase page, string tag, string info, params string[] classes)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentNullException("tag");

            TagBuilder tagBuilder = new TagBuilder(tag);
            tagBuilder.Attributes["data-virtualpath"] = VirtualPathUtility.ToAbsolute(info).ToLowerInvariant();

            foreach (var @class in classes)
            {
                tagBuilder.AddCssClass(@class);
            }

            return new Chunk(page, tagBuilder);
        }

        public class Chunk : IDisposable
        {
            private readonly WebPageBase _page;
            private readonly TagBuilder _tagBuilder;

            public Chunk(WebPageBase page, TagBuilder tagBuilder)
            {
                _page = page;
                _tagBuilder = tagBuilder;


                if (tagBuilder == null) return;

                page.Output.WriteLine(Environment.NewLine + tagBuilder.ToString(TagRenderMode.StartTag));
                tagBuilder.ToString(TagRenderMode.Normal);
            }

            public void Dispose()
            {
                if (_tagBuilder == null) return;

                _page.Output.WriteLine(Environment.NewLine + _tagBuilder.ToString(TagRenderMode.EndTag));
            }
        }
    }
}