using System.Configuration;

namespace Frankstein.Common.Configuration
{
    public class DumpToLocalElement : BooleanElementBase
    {
        [ConfigurationProperty("folder", DefaultValue = "~/App_Code")]
        public string Folder
        {
            get { return (string)this["folder"]; }
            set { this["folder"] = value; }
        }

        [ConfigurationProperty("deletefiles", DefaultValue = false)]
        public bool DeleteFiles
        {
            get { return (bool)this["deletefiles"]; }
            set { this["deletefiles"] = value; }
        }

        [ConfigurationProperty("sync", DefaultValue = false)]
        public bool Sync
        {
            get { return (bool)this["sync"]; }
            set { this["sync"] = value; }
        }
    }
}