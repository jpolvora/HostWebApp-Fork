using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Threading;
using Frankstein.Common;
using Frankstein.Common.Configuration;
using Frankstein.DbFileSystem.EF;

namespace Frankstein.DbFileSystem
{
    public class DbFileContext : DbContext
    {
        private static bool _initialized;

        protected string Modifier { get; private set; }

        public DbSet<DbFile> DbFiles { get; set; }

        public static string ConnectionStringKey { get; private set; }
        public static bool Verbose { get; private set; }

        public static void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;

            using (var db = new DbFileContext())
            {
                Trace.TraceInformation("Connection String: {0}", db.Database.Connection.ConnectionString);
                db.Database.Initialize(false);
            }
        }

        static DbFileContext()
        {
            ConnectionStringKey = BootstrapperSection.Instance.DbFileContext.ConnectionStringKey;
            Verbose = BootstrapperSection.Instance.DbFileContext.Verbose;
        }

        public DbFileContext()
            : this(ConnectionStringKey)
        {
        }

        public DbFileContext(string connStrKey)
            : base(connStrKey)
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;

            Modifier = Thread.CurrentPrincipal != null ? Thread.CurrentPrincipal.Identity.Name : "";

            if (Verbose)
            {
                Database.Log = Log;
            }
        }

        static void Log(string str)
        {
            if (str.StartsWith("-- Completed"))
            {
                Trace.Indent();
                Trace.TraceInformation("[DbFileContext]:{0}", str.Replace(Environment.NewLine, ""));
                Trace.Unindent();
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Conventions.Add(new MapToCharAttributeConvention());
            modelBuilder.Conventions.Add(new DecimalMappingAttributeConvention());

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            var auditables = ChangeTracker.Entries<AuditableEntity>();
            foreach (var auditable in auditables)
            {
                switch (auditable.State)
                {
                    case EntityState.Added:
                        auditable.Entity.Created = DateTime.UtcNow;
                        auditable.Entity.Modified = null;
                        break;
                    case EntityState.Modified:
                        auditable.Property(x => x.Created).IsModified = false;
                        auditable.Entity.Modified = DateTime.UtcNow;
                        break;
                }

                auditable.Entity.Modifier = Modifier.Truncate(100);
            }

            return base.SaveChanges();
        }
    }
}