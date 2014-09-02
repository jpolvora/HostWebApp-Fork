using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcLib.DbFileSystem;

namespace HostWebApp.Migrations
{
    public class Configuration : DbFileContextMigrationConfiguration
    {
        public Configuration()
        {
            ContextKey = GetType().BaseType.Name;
        }
    }
}