using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Frankstein.Bootstrapper;
using Frankstein.Common;
using Frankstein.Common.Cache;
using Frankstein.Common.Configuration;
using Frankstein.Common.Mvc;
using Frankstein.DbFileSystem;
using Frankstein.FsDump;
using Frankstein.HttpModules;
using Frankstein.Kompiler;
using Frankstein.PluginLoader;
using Frankstein.VirtualPathProviders;
using Frankstein.VirtualPathProviders.Impl;
using Frankstein.VirtualPathProviders.RemapperVpp;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Init), "PreStart")]
[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(Init), "PostStart")]

namespace Frankstein.Bootstrapper
{
    public class Init
    {
        private static string _traceFileName;
        private static bool _initialized;

        public static void PreStart()
        {
            var cfg = BootstrapperSection.Initialize();

            //cria um text logger somente para o startup
            //remove no post start
            try
            {
                _traceFileName = HostingEnvironment.MapPath(cfg.TraceOutput);
                if (!string.IsNullOrWhiteSpace(_traceFileName))
                {
                    if (File.Exists(_traceFileName))
                        try
                        {
                            File.Delete(_traceFileName);
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(ex.ToString());
                        }

                    var listener = new TextWriterTraceListener(_traceFileName, "StartupListener");

                    Trace.Listeners.Add(listener);

                    Trace.TraceInformation("[Bootstrapper]: StartupLog added: {0}", listener);
                }
            }
            catch
            {
            }

            using (DisposableTimer.StartNew("Frankstein PRE_START"))
            {
                var executingAssembly = Assembly.GetExecutingAssembly();
                Trace.TraceInformation("[Bootstrapper]:Entry Assembly: {0}", executingAssembly.GetName().Name);

                DynamicModuleUtility.RegisterModule(typeof(FranksteinHttpModule));

                Trace.TraceInformation("[Bootstrapper]:cfg.HttpModules.Trace.Enabled = {0}", cfg.HttpModules.Trace.Enabled);
                if (cfg.HttpModules.Trace.Enabled)
                {
                    DynamicModuleUtility.RegisterModule(typeof(TracerHttpModule));
                }

                Trace.TraceInformation("[Bootstrapper]:cfg.HttpModules.TransactionScope.Enabled = {0}", cfg.HttpModules.TransactionScope.Enabled);
                if (cfg.HttpModules.TransactionScope.Enabled)
                {
                    DynamicModuleUtility.RegisterModule(typeof(TransactionScopeHttpModule));
                }

                Trace.TraceInformation("[Bootstrapper]:cfg.StopMonitoring = {0}", cfg.StopMonitoring);
                if (cfg.StopMonitoring)
                {
                    HttpInternals.StopFileMonitoring();
                }

                Trace.TraceInformation("[Bootstrapper]:cfg.HttpModules.CustomError.Enabled = {0}", cfg.HttpModules.CustomError.Enabled);
                if (cfg.HttpModules.CustomError.Enabled)
                {
                    DynamicModuleUtility.RegisterModule(typeof(CustomErrorHttpModule));
                }

                Trace.TraceInformation("[Bootstrapper]:cfg.HttpModules.WhiteSpace.Enabled = {0}", cfg.HttpModules.WhiteSpace.Enabled);
                if (cfg.HttpModules.WhiteSpace.Enabled)
                {
                    DynamicModuleUtility.RegisterModule(typeof(WhitespaceModule));
                }

                Trace.TraceInformation("[Bootstrapper]:cfg.HttpModules.PathRewriter.Enabled = {0}", cfg.HttpModules.PathRewriter.Enabled);
                if (cfg.HttpModules.PathRewriter.Enabled)
                {
                    DynamicModuleUtility.RegisterModule(typeof(PathRewriterHttpModule));
                }

                using (DisposableTimer.StartNew("DbFileContext"))
                {
                    DbFileContext.Initialize();
                }

                Trace.TraceInformation("[Bootstrapper]:cfg.PluginLoader.Enabled = {0}", cfg.PluginLoader.Enabled);
                if (cfg.PluginLoader.Enabled)
                {
                    using (DisposableTimer.StartNew("PluginLoader"))
                    {
                        PluginLoaderEntryPoint.Initialize();
                    }
                }

                Trace.TraceInformation("[Bootstrapper]:cfg.VirtualPathProviders.SubFolderVpp.Enabled = {0}", cfg.VirtualPathProviders.SubFolderVpp.Enabled);
                if (cfg.VirtualPathProviders.SubFolderVpp.Enabled)
                {
                    SubfolderVpp.SelfRegister();
                }

                Trace.TraceInformation("[Bootstrapper]:cfg.VirtualPathProviders.DbFileSystemVpp.Enabled = {0}", cfg.VirtualPathProviders.DbFileSystemVpp.Enabled);
                if (cfg.VirtualPathProviders.DbFileSystemVpp.Enabled)
                {
                    var customvpp = new CustomVirtualPathProvider()
                        .AddImpl(new CachedDbServiceFileSystemProvider(new DefaultDbService(), new WebCacheWrapper()));
                    HostingEnvironment.RegisterVirtualPathProvider(customvpp);
                }

                Trace.TraceInformation("[Bootstrapper]:cfg.DumpToLocal.Enabled = {0}", cfg.DumpToLocal.Enabled);
                if (cfg.DumpToLocal.Enabled)
                {
                    using (DisposableTimer.StartNew("DumpToLocal"))
                    {
                        DbToLocal.Execute();
                    }
                }

                Trace.TraceInformation("[Bootstrapper]:cfg.Kompiler.Enabled = {0}", cfg.Kompiler.Enabled);
                var bin = new DirectoryInfo(HttpRuntime.BinDirectory);

                Trace.Indent();

                foreach (var fileInfo in bin.EnumerateFiles("*.dll", SearchOption.AllDirectories))
                {
                    if (cfg.Verbose)
                    {
                        Trace.TraceInformation("[BinFolder]: {0}", fileInfo.FullName);
                    }
                    KompilerEntryPoint.AddReferences(fileInfo.FullName);
                }
                Trace.Unindent();

                if (cfg.Kompiler.Enabled)
                {
                    using (DisposableTimer.StartNew("Kompiler"))
                    {
                        KompilerEntryPoint.Execute();
                    }
                }
            }
        }

        public static void PostStart()
        {
            if (_initialized)
                return;

            _initialized = true;

            var cfg = BootstrapperSection.Instance;
            using (DisposableTimer.StartNew("Frankstein POST_START..."))
            {
                Trace.TraceInformation("[Bootstrapper]:Debugging Enabled: {0}", HttpContext.Current.IsDebuggingEnabled);
                Trace.TraceInformation("[Bootstrapper]:CustomErrors Enabled: {0}", HttpContext.Current.IsCustomErrorEnabled);
                var commitId = Config.ValueOrDefault("appharbor.commit_id", "");
                Trace.TraceInformation("[Bootstrapper]:Commit Id: {0}", commitId);

                Trace.TraceInformation("[Bootstrapper]:cfg.MvcTrace.Enabled = {0}", cfg.MvcTrace.Enabled);
                if (cfg.MvcTrace.Enabled)
                {
                    GlobalFilters.Filters.Add(new MvcTracerFilter());
                }

                Trace.TraceInformation("[Bootstrapper]:cfg.InsertRoutes = {0}", cfg.InsertRoutes);
                if (cfg.InsertRoutes)
                {
                    var routes = RouteTable.Routes;

                    routes.RouteExistingFiles = false;
                    routes.LowercaseUrls = true;
                    routes.AppendTrailingSlash = true;

                    routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
                    //routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
                    //routes.IgnoreRoute("{*staticfile}", new { staticfile = @".*\.(css|js|txt|png|gif|jpg|jpeg|bmp)(/.*)?" });
                    routes.IgnoreRoute("{*staticfile}", new { staticfile = @".*\.(?i)(css|js|xml|txt|png|gif|jpg|jpeg|bmp|ico|woff|svg|ttf|eot)(/.*)?" });
                    routes.IgnoreRoute("Content/{*pathInfo}");
                    routes.IgnoreRoute("Scripts/{*pathInfo}");
                    routes.IgnoreRoute("Bundles/{*pathInfo}");

                    routes.MapMvcAttributeRoutes();
                }

                Trace.TraceInformation("[Bootstrapper]:cfg.Verbose = {0}", cfg.Verbose);
                if (cfg.Verbose)
                {
                    var application = HttpContext.Current.ApplicationInstance;

                    var modules = application.Modules;
                    Trace.Indent();
                    foreach (var module in modules)
                    {
                        Trace.TraceInformation("Module Loaded: {0}", module);
                    }
                    Trace.Unindent();

                    //dump routes
                    var routes = RouteTable.Routes;


                    var i = routes.Count;
                    Trace.TraceInformation("[Bootstrapper]:Found {0} routes in RouteTable", i);
                    Trace.Indent();
                    foreach (var routeBase in routes)
                    {
                        var route = routeBase as Route;
                        if (route != null)
                        {
                            Trace.TraceInformation("Handler: {0} at URL: {1}", route.RouteHandler, route.Url);
                        }
                        else
                        {
                            //RouteCollectionRoute => mapped by AttributeRouting
                            //it's internal, so it's elligible to access his methods and properties with ImpromptuInterface
                            Trace.TraceInformation("route: {0}", routeBase);
                        }
                    }
                    Trace.Unindent();
                }

                //viewengine locations
                var mvcroot = cfg.DumpToLocal.Folder;

                var razorViewEngine = ViewEngines.Engines.OfType<RazorViewEngine>().FirstOrDefault();
                if (razorViewEngine != null)
                {
                    Trace.TraceInformation("[Bootstrapper]:Configuring RazorViewEngine Location Formats");
                    var vlf = new string[]
                    {
                        mvcroot + "/Views/{1}/{0}.cshtml",
                        mvcroot + "/Views/Shared/{0}.cshtml",
                    };
                    razorViewEngine.ViewLocationFormats = razorViewEngine.ViewLocationFormats.Extend(false, vlf);

                    var mlf = new string[]
                    {
                        mvcroot + "/Views/{1}/{0}.cshtml",
                        mvcroot + "/Views/Shared/{0}.cshtml",
                    };
                    razorViewEngine.MasterLocationFormats = razorViewEngine.MasterLocationFormats.Extend(false, mlf);

                    var plf = new string[]
                    {
                        mvcroot + "/Views/{1}/{0}.cshtml",
                        mvcroot + "/Views/Shared/{0}.cshtml",
                    };
                    razorViewEngine.PartialViewLocationFormats = razorViewEngine.PartialViewLocationFormats.Extend(false, plf);

                    var avlf = new string[]
                    {
                        mvcroot + "/Areas/{2}/Views/{1}/{0}.cshtml",
                        mvcroot + "/Areas/{2}/Views/Shared/{0}.cshtml",
                    };
                    razorViewEngine.AreaViewLocationFormats = razorViewEngine.AreaViewLocationFormats.Extend(false, avlf);

                    var amlf = new string[]
                    {
                        mvcroot + "/Areas/{2}/Views/{1}/{0}.cshtml",
                        mvcroot + "/Areas/{2}/Views/Shared/{0}.cshtml",
                    };
                    razorViewEngine.AreaMasterLocationFormats = razorViewEngine.AreaMasterLocationFormats.Extend(false, amlf);

                    var apvlf = new string[]
                    {
                        mvcroot + "/Areas/{2}/Views/{1}/{0}.cshtml",
                        mvcroot + "/Areas/{2}/Views/Shared/{0}.cshtml",
                    };
                    razorViewEngine.AreaPartialViewLocationFormats = razorViewEngine.AreaPartialViewLocationFormats.Extend(false, apvlf);

                    if (cfg.Verbose)
                    {
                        Trace.Indent();
                        foreach (var locationFormat in razorViewEngine.ViewLocationFormats)
                        {
                            Trace.TraceInformation(locationFormat);
                        }
                        Trace.Unindent();
                    }

                    ViewEngines.Engines.Clear();
                    ViewEngines.Engines.Add(razorViewEngine);
                }
                else
                {
                    Trace.TraceInformation("[Bootstrapper]:Cannot Configure RazorViewEngine: View Engine not found");
                }
            }

            Trace.Flush();

            if (cfg.Verbose)
            {
                Trace.TraceInformation("Listing Attached TraceListeners...");
                Trace.Indent();

                var listeners = Trace.Listeners;
                foreach (var logger in listeners)
                {
                    Trace.TraceInformation(logger.ToString());
                }
                Trace.Unindent();
            }

            var listener = Trace.Listeners["StartupListener"] as TextWriterTraceListener;
            if (listener != null)
            {
                listener.Flush();
                listener.Close();
                Trace.Listeners.Remove(listener);
            }

            //envia log de startup por email
            Trace.TraceInformation("[Bootstrapper]:cfg.Mail.SendStartupLog = {0}", cfg.Mail.SendStartupLog);
            if (cfg.Mail.SendStartupLog && !Config.IsInDebugMode)
            {
                if (!File.Exists(_traceFileName)) return;
                ThreadPool.QueueUserWorkItem(state =>
                {
                    var body = File.ReadAllText(_traceFileName);

                    Trace.TraceInformation("[Bootstrapper]:Sending startup log email to {0}", cfg.Mail.MailDeveloper);

                    EmailExtensions.SendEmail(new MailAddress(cfg.Mail.MailAdmin, "Admin"),
                        new MailAddress(cfg.Mail.MailDeveloper, "Developer"),
                        string.Format("App Startup Log: {0} at {1} ", cfg.AppName, DateTime.Now),
                        body,
                        true,
                        (msg, ex) =>
                        {
                            if (ex != null)
                            {
                                Trace.TraceError("[Bootstrapper]:Error sending startup log email: {0}", ex.Message);
                            }
                            else
                            {
                                Trace.TraceInformation("[Bootstrapper]:Startup log was sent successfully to {0}", msg.To[0]);
                            }
                        });
                });
            }
        }
    }
}