using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using MvcLib.Common;
using MvcLib.Common.Cache;
using MvcLib.Common.Configuration;
using MvcLib.Common.Mvc;
using MvcLib.CustomVPP;
using MvcLib.CustomVPP.Impl;
using MvcLib.CustomVPP.RemapperVpp;
using MvcLib.DbFileSystem;
using MvcLib.FsDump;
using MvcLib.HttpModules;
using MvcLib.PluginLoader;
using EntryPoint = MvcLib.Bootstrapper.EntryPoint;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(EntryPoint), "PreStart")]
[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(EntryPoint), "PostStart")]

namespace MvcLib.Bootstrapper
{
    public class EntryPoint
    {
        private static bool _initialized;

        public static void PreStart()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            Trace.TraceInformation("Entry Assembly: {0}", executingAssembly.GetName().Name);

            if (Debugger.IsAttached)
            {
                try
                {
                    var traceOutput = HostingEnvironment.MapPath("~/traceOutput.log");
                    if (File.Exists(traceOutput))
                        File.Delete(traceOutput);

                    var listener = new TextWriterTraceListener(traceOutput, "StartupListener");

                    Trace.Listeners.Add(listener);
                    Trace.AutoFlush = true;
                }
                catch { }
            }

            BootstrapperSection.Initialize();

            using (DisposableTimer.StartNew("PRE_START: Configuring HttpModules"))
            {
                if (BootstrapperSection.Instance.HttpModules.Trace.Enabled)
                {
                    DynamicModuleUtility.RegisterModule(typeof(TracerHttpModule));
                }

                if (BootstrapperSection.Instance.StopMonitoring)
                {
                    HttpInternals.StopFileMonitoring();
                }

                if (BootstrapperSection.Instance.HttpModules.CustomError.Enabled)
                {
                    DynamicModuleUtility.RegisterModule(typeof(CustomErrorHttpModule));
                }

                if (BootstrapperSection.Instance.HttpModules.WhiteSpace.Enabled)
                {
                    DynamicModuleUtility.RegisterModule(typeof(WhitespaceModule));
                }

                using (DisposableTimer.StartNew("DbFileContext"))
                {
                    DbFileContext.Initialize();
                }

                //plugin loader deve ser utilizado se dump to local = true ou se utilizar o custom vpp
                if (BootstrapperSection.Instance.PluginLoader.Enabled)
                {
                    using (DisposableTimer.StartNew("PluginLoader"))
                    {
                        PluginLoader.EntryPoint.Initialize();
                    }
                }

                if (BootstrapperSection.Instance.VirtualPathProviders.SubFolderVpp.Enabled)
                {
                    var customvpp = new SubfolderVpp();
                    HostingEnvironment.RegisterVirtualPathProvider(customvpp);
                }

                if (BootstrapperSection.Instance.DumpToLocal.Enabled)
                {
                    using (DisposableTimer.StartNew("DumpToLocal"))
                    {
                        DbToLocal.Execute();
                    }
                }

                //todo: Dependency Injection
                if (BootstrapperSection.Instance.VirtualPathProviders.DbFileSystemVpp.Enabled)
                {
                    var customvpp = new CustomVirtualPathProvider()
                        .AddImpl(new CachedDbServiceFileSystemProvider(new DefaultDbService(), new WebCacheWrapper()));
                    HostingEnvironment.RegisterVirtualPathProvider(customvpp);
                }

                //todo: implementar dependência entre módulos

                if (BootstrapperSection.Instance.Kompiler.Enabled)
                {
                    if (BootstrapperSection.Instance.Kompiler.ForceRecompilation)
                    {
                        //se forçar a recompilação, remove o assembly existente.
                        Kompiler.KompilerDbService.RemoveExistingCompiledAssemblyFromDb();
                    }

                    //se já houver um assembly compilado, não executa a compilação
                    if (!Kompiler.KompilerDbService.ExistsCompiledAssembly())
                    {
                        //EntryPoint depends on PluginLoader, so, initializes it if not previously initialized.
                        PluginLoader.EntryPoint.Initialize();

                        Kompiler.EntryPoint.AddReferences(typeof(Controller), typeof(WebPageRenderingBase), typeof(WebCacheWrapper), typeof(ViewRenderer), typeof(DbToLocal), typeof(CustomErrorHttpModule.ErrorModel));

                        using (DisposableTimer.StartNew("Kompiler"))
                        {
                            Kompiler.EntryPoint.Execute();
                        }
                    }
                }

                //config routing
                //var routes = RouteTable.Routes;

                //if (EntropiaSection.Instance.InsertRoutesDefaults)
                //{
                //    routes.RouteExistingFiles = false;
                //    routes.LowercaseUrls = true;
                //    routes.AppendTrailingSlash = true;

                //    routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
                //    routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
                //    routes.IgnoreRoute("{*staticfile}", new { staticfile = @".*\.(css|js|txt|png|gif|jpg|jpeg|bmp)(/.*)?" });

                //    routes.IgnoreRoute("Content/{*pathInfo}");
                //    routes.IgnoreRoute("Scripts/{*pathInfo}");
                //    routes.IgnoreRoute("Bundles/{*pathInfo}");
                //}

                //if (EntropiaSection.Instance.EnableDumpLog)
                //{
                //    var endpoint = EntropiaSection.Instance.DumpLogEndPoint;
                //    routes.MapHttpHandler<DumpHandler>(endpoint);
                //}
            }
        }

        public static void PostStart()
        {
            if (_initialized)
                return;

            _initialized = true;

            using (DisposableTimer.StartNew("RUNNING POST_START ..."))
            {
                if (BootstrapperSection.Instance.MvcTrace.Enabled)
                {
                    GlobalFilters.Filters.Add(new MvcTracerFilter());
                }

                var application = HttpContext.Current.ApplicationInstance;

                var modules = application.Modules;
                foreach (var module in modules)
                {
                    Trace.TraceInformation("Module Loaded: {0}", module);
                }

                //dump routes
                var routes = RouteTable.Routes;

                var i = routes.Count;
                Trace.TraceInformation("Found {0} routes in RouteTable", i);

                foreach (var routeBase in routes)
                {
                    var route = (Route)routeBase;
                    Trace.TraceInformation("Handler: {0} at URL: {1}", route.RouteHandler, route.Url);
                }

                if (!Debugger.IsAttached)
                {
                    Trace.Listeners.Remove("StartupListener");
                }
            }
        }
    }
}