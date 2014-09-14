using System.Configuration;

namespace Frankstein.Common.Configuration
{
    public class PathRewriter : BooleanElementBase
    {
        [ConfigurationProperty("ignorepaths", DefaultValue = "")]
        public string IgnorePaths
        {
            get { return (string)this["ignorepaths"]; }
            set { this["ignorepaths"] = value; }
        }
    }
}