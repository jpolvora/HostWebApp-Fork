using System;
using System.Configuration;

namespace MvcLib.Common.Configuration
{
    public class BootstrapperSection : ConfigurationSection
    {
        public static void Initialize()
        {
        }

        static BootstrapperSection()
        {
            Instance = (BootstrapperSection)ConfigurationManager.GetSection("MvcLib");
        }

        public static BootstrapperSection Instance { get; private set; }

        [ConfigurationProperty("stopMonitoring", DefaultValue = "true")]
        public Boolean StopMonitoring
        {
            get
            {
                return (Boolean)this["stopMonitoring"];
            }
            set
            {
                this["stopMonitoring"] = value;
            }
        }

        [ConfigurationProperty("httpmodules")]
        public HttpModulesElement HttpModules
        {
            get { return (HttpModulesElement)this["httpmodules"]; }
            set { this["httpmodules"] = value; }
        }

        [ConfigurationProperty("mvctrace")]
        public MvcTraceElement MvcTrace
        {
            get { return (MvcTraceElement)this["mvctrace"]; }
            set { this["mvctrace"] = value; }
        }

        [ConfigurationProperty("dbfilecontext")]
        public DbFileContextElement DbFileContext
        {
            get { return (DbFileContextElement)this["dbfilecontext"]; }
            set { this["dbfilecontext"] = value; }
        }

        [ConfigurationProperty("dumptolocal")]
        public DumpToLocalElement DumpToLocal
        {
            get { return (DumpToLocalElement)this["dumptolocal"]; }
            set { this["dumptolocal"] = value; }
        }

        [ConfigurationProperty("kompiler")]
        public KompilerElement Kompiler
        {
            get { return (KompilerElement)this["kompiler"]; }
            set { this["kompiler"] = value; }
        }

        [ConfigurationProperty("pluginloader")]
        public PluginLoaderElementBase PluginLoader
        {
            get { return (PluginLoaderElementBase)this["pluginloader"]; }
            set { this["pluginloader"] = value; }
        }

        [ConfigurationProperty("virtualpathproviders")]
        public VirtualPathProviderElement VirtualPathProviders
        {
            get { return (VirtualPathProviderElement)this["virtualpathproviders"]; }
            set { this["virtualpathproviders"] = value; }
        }
    }
}