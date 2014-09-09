using Frankstein.Common.Configuration;
using Frankstein.DbFileSystem;

namespace HostWebApp.Migrations
{
    /// <summary>
    /// Esta classe só pode ser executada via Update-Database no Package Manager Console.
    /// </summary>
    public class Configuration : DbFileContextMigrationConfiguration
    {
        public Configuration()
        {
            ContextKey = BootstrapperSection.Instance.DbFileContext.MigrationKey;
        }
    }
}