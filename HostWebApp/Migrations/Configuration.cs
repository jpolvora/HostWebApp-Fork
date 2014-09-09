using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcLib.Common.Configuration;
using MvcLib.DbFileSystem;

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