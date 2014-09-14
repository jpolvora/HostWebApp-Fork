using System;
using System.Security.Principal;

namespace Frankstein.Common.Mvc.Authentication
{
    public class DbPrincipal<T> : IPrincipal where T : class, IDbUser, new()
    {
        private readonly IPrincipal _principal;
        public T DbUser { get; private set; }

        public static implicit operator T(DbPrincipal<T> instance)
        {
            return instance.DbUser;
        }

        public DbPrincipal(IIdentity identity, T dbUser)
        {
            if (identity == null)
                throw new ArgumentNullException("identity");

            if (dbUser == null)
                throw new ArgumentNullException("dbUser");

            var roles = dbUser.GetRoles().ToArray();
            _principal = new GenericPrincipal(identity, roles);

            DbUser = dbUser;
        }

        public bool IsInRole(string role)
        {
            return _principal.IsInRole(role);
        }

        public IIdentity Identity
        {
            get { return _principal.Identity; }
        }
    }
}