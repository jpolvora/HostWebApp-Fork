using System.Configuration;

namespace MvcLib.Common.Configuration
{
    public class TraceElement : ConfigurationElement
    {
        [ConfigurationProperty("enabled", DefaultValue = false)]
        public bool Enabled
        {
            get { return (bool)this["enabled"]; }
            set { this["enabled"] = value; }
        }

        [ConfigurationProperty("events", DefaultValue = "")]
        public string Events
        {
            get { return (string)this["events"]; }
            set { this["events"] = value; }
        }
    }
}