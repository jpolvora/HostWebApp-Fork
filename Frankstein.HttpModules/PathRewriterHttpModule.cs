using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Frankstein.Common.Configuration;

namespace Frankstein.HttpModules
{
    public class PathRewriterHttpModule : IHttpModule
    {
        private static readonly string _rewriteBasePath;

        static PathRewriterHttpModule()
        {
            _rewriteBasePath = BootstrapperSection.Instance.DumpToLocal.Folder.TrimEnd('/');
            if (!_rewriteBasePath.StartsWith("~"))
                _rewriteBasePath = "~" + _rewriteBasePath;

            Trace.TraceInformation("[PathRewriterHttpModule]: '{0}'", _rewriteBasePath);
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += (s, e) => ContextOnBeginRequest(context);
        }

        private static void ContextOnBeginRequest(HttpApplication application)
        {
            string path = application.Request.Url.AbsolutePath.TrimEnd('/');

            //se o path nÃ£o precisa ser reescrito
            if (path.StartsWith(_rewriteBasePath.Substring(1)))
                return;

            string virtualPath = string.Format("{0}/{1}", _rewriteBasePath, path);

            var virtualFileName = VirtualPathUtility.GetFileName(virtualPath);
            var extension = VirtualPathUtility.GetExtension(virtualFileName);
            if (string.IsNullOrWhiteSpace(extension))
            {
                const string defaultcshtml = "default.cshtml";
                var found = false;
                //compatibilidade com webpages url routing
                var segments = application.Request.Url.Segments.Reverse();
                foreach (var segment in segments)
                {
                    var file = segment.Trim('/') + ".cshtml";
                    if (file.Length < 7)
                        file = defaultcshtml;
                    
                    virtualPath = string.Format("{0}/{1}", _rewriteBasePath, file);
                    if (HostingEnvironment.VirtualPathProvider.FileExists(virtualPath))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    virtualPath = string.Format("{0}/{1}", _rewriteBasePath, defaultcshtml);
                    if (HostingEnvironment.VirtualPathProvider.FileExists(virtualPath))
                    {
                        Trace.TraceInformation("[PathRewriterHttpModule]:Rewriting path from '{0}' to '{1}'", path,
                            virtualPath);
                        application.Context.RewritePath(virtualPath);
                    }
                    return; //fallback to mvc
                }
                Trace.TraceInformation("[PathRewriterHttpModule]:Rewriting path from '{0}' to '{1}'", path,
                    virtualPath);
                application.Context.RewritePath(virtualPath);
            }

            if (!HostingEnvironment.VirtualPathProvider.FileExists(virtualPath)) return; //fallback to mvc

            string newpath = string.Format("{0}{1}", _rewriteBasePath.Substring(1), path);

            Trace.TraceInformation("[PathRewriterHttpModule]:Rewriting path from '{0}' to '{1}'", path, newpath);
            application.Context.RewritePath(newpath);
        }

        public void Dispose()
        {
        }
    }
}