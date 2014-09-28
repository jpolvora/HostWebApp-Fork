using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Web.Hosting;

namespace Frankstein.Common.Configuration
{
    public static class CustomConfig
    {
        public static IEnumerable<string> RefreshConfig()
        {
            var virtualPath = BootstrapperSection.Instance.DumpToLocal.Folder + "/kompiler.config";
            var phisycalPath = HostingEnvironment.MapPath(virtualPath);

            var roamingConfig = new FileInfo(phisycalPath);
            if (!roamingConfig.Exists)
            {
                yield break;
            }

            var configFileMap = new ExeConfigurationFileMap { ExeConfigFilename = roamingConfig.FullName };

            // Get the mapped configuration file.
            var config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            var section = config.GetSection("kompiler") as AppSettingsSection;
            if (section != null)
            {
                foreach (KeyValueConfigurationElement setting in section.Settings)
                {
                    yield return setting.Value;
                }
            }
        }
    }
}