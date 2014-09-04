using System.Configuration;

namespace MvcLib.Common.Configuration
{
    public class HttpModulesElement : ConfigurationElement
    {
        [ConfigurationProperty("trace", DefaultValue = false)]
        public bool Trace
        {
            get { return (bool)this["trace"]; }
            set { this["trace"] = value; }
        }
    }
}