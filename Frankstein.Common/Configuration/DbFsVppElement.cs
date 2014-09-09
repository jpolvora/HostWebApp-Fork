using System.Configuration;

namespace Frankstein.Common.Configuration
{
    public class DbFsVppElement : BooleanElementBase
    {
        [ConfigurationProperty("usecachewrapper", DefaultValue = false)]
        public bool UseCacheWrapper
        {
            get { return (bool)this["usecachewrapper"]; }
            set { this["usecachewrapper"] = value; }
        }
    }
}