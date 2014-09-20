using System.Data.Entity;
using System.Diagnostics;
using Frankstein.EntityFramework;

namespace Frankstein.DbFileSystem
{
    public class DbFileContext : DbContextBase
    {
        private static bool _initialized;

        public DbSet<DbFile> DbFiles { get; set; }

        public static void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;

            using (var db = new DbFileContext())
            {
                Trace.TraceInformation("Initializing DbFileContext: {0}", db.Database.Connection.ConnectionString);
                db.Database.Initialize(false);
            }
        }
    }
}