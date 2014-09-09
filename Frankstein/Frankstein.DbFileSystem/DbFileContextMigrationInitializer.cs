using System.Data.Entity;

namespace Frankstein.DbFileSystem
{
    public class DbFileContextMigrationInitializer : MigrateDatabaseToLatestVersion<DbFileContext, DbFileContextMigrationConfiguration>
    {
        
    }
}