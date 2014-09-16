using System.Configuration;

namespace Frankstein.Common.Configuration
{
    public class TraceElement : BooleanElementBase
    {
        [ConfigurationProperty("events", DefaultValue = "")]
        public string Events
        {
            get { return (string)this["events"]; }
            set { this["events"] = value; }
        }

        [ConfigurationProperty("verbose", DefaultValue = false)]
        public bool Verbose
        {
            get { return (bool)this["verbose"]; }
            set { this["verbose"] = value; }
        }

        [ConfigurationProperty("bufferize", DefaultValue = false)]
        public bool Bufferize
        {
            get { return (bool)this["bufferize"]; }
            set { this["bufferize"] = value; }
        }
    }
}