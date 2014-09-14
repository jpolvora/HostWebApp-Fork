using System.Security.Principal;

namespace Frankstein.Common.Mvc.Authentication
{
    public static class PrincipalExtensions
    {
        public static DbPrincipal<T> FromCurrent<T>(this IPrincipal principal) where T : class, IDbUser, new()
        {
            if (principal is DbPrincipal<T>)
                return (DbPrincipal<T>)principal;

            return new DbPrincipal<T>(principal.Identity, new T());
        }
    }
}