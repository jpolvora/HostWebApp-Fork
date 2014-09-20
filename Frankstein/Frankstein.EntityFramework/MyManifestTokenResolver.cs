using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace Frankstein.EntityFramework
{
    public class MyManifestTokenResolver : IManifestTokenResolver
    {
        private readonly IManifestTokenResolver _defaultResolver = new DefaultManifestTokenResolver();

        public string ResolveManifestToken(DbConnection connection)
        {
            var sqlConn = connection as SqlConnection;
            return sqlConn != null
                ? "2008"
                : _defaultResolver.ResolveManifestToken(connection);
        }
    }
}