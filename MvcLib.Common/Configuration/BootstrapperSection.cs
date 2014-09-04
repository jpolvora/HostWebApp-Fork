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

        [ConfigurationProperty("stopMonitoring", DefaultValue = "false")]
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

        [ConfigurationProperty("trace")]
        public TraceElement Trace
        {
            get { return (TraceElement)this["trace"]; }
            set { this["trace"] = value; }
        }
    }
}