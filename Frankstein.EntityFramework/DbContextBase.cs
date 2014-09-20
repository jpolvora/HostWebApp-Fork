using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Threading;
using Frankstein.Common.Configuration;

namespace Frankstein.EntityFramework
{
    public abstract class DbContextBase : DbContext
    {
        public static string ConnectionStringKey { get; private set; }
        public static bool Verbose { get; private set; }

        static DbContextBase()
        {
            ConnectionStringKey = BootstrapperSection.Instance.DbFileContext.ConnectionStringKey;
            Verbose = BootstrapperSection.Instance.DbFileContext.Verbose;
        }

        protected DbContextBase()
            : base(ConnectionStringKey)
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;

            if (Verbose)
            {
                Database.Log = Log;
            }
        }

        static void Log(string str)
        {
            if (!str.StartsWith("-- Completed")) return;
            Trace.Indent();
            Trace.TraceInformation("[DbContextBase]:{0}", str.Replace(Environment.NewLine, ""));
            Trace.Unindent();
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

                auditable.Entity.Modifier = Thread.CurrentPrincipal != null ? Thread.CurrentPrincipal.Identity.Name : "";
            }

            return base.SaveChanges();
        }
    }
}