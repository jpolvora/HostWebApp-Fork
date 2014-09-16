using System.Configuration;

namespace Frankstein.Common.Configuration
{
    public class BooleanElementBase : ConfigurationElement
    {
        [ConfigurationProperty("enabled", DefaultValue = false)]
        public bool Enabled
        {
            get { return (bool)this["enabled"]; }
            set { this["enabled"] = value; }
        }
    }
}