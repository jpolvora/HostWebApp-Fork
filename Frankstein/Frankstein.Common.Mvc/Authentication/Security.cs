using System.Net;
using System.Web;
using Antlr.Runtime.Misc;
using Frankstein.Common.Configuration;

namespace Frankstein.Common.Mvc.Authentication
{
    public static class Security
    {
        /// <summary>
        /// retorna coom 401
        /// </summary>
        public static void RequiresAuthenticatedUser()
        {
            if (!BootstrapperSection.Instance.SecurityEnabled)
                return;

            var context = HttpContext.Current;


            if (!context.Request.IsAuthenticated)
            {
                context.Response.StatusCode = 401;
                context.Response.End();
            }
        }

        /// <summary>
        /// retorna com 403
        /// </summary>
        /// <param name="roles"></param>
        public static void RequiresRoles(params string[] roles)
        {
            if (!BootstrapperSection.Instance.SecurityEnabled)
                return;

            RequiresAuthenticatedUser();

            var context = HttpContext.Current;
            var user = context.User;
            foreach (var role in roles)
            {
                if (!user.IsInRole(role))
                {
                    context.Response.StatusCode = 403;
                    context.Response.End();
                    break;
                }
            }
        }

        public static void RequiresCondition<T>(Func<T, bool> condition) where T : class, IDbUser, new()
        {
            if (!BootstrapperSection.Instance.SecurityEnabled)
                return;

            RequiresAuthenticatedUser();

            var context = HttpContext.Current;

            var user = context.User.FromCurrent<T>();

            if (condition(user)) return;

            context.Response.StatusCode = 403;
            context.Response.End();
        }
    }
}