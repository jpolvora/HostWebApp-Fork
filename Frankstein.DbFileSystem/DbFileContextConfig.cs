using System.Data.Entity;
using Frankstein.EntityFramework;

namespace Frankstein.DbFileSystem
{
    public class DbFileContextConfig : DbConfiguration
    {
        public DbFileContextConfig()
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<DbFileContext, DbFileContextMigrationConfiguration>());
            //SetDatabaseInitializer<DbFileContext>(null);
            SetManifestTokenResolver(new MyManifestTokenResolver());
        }
    }
}