using System.Net;
using System.Web;
using Antlr.Runtime.Misc;

namespace Frankstein.Common.Mvc.Authentication
{
    public static class Security
    {
        /// <summary>
        /// retorna coom 401
        /// </summary>
        public static void RequiresAuthenticatedUser()
        {
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
            RequiresAuthenticatedUser();

            var context = HttpContext.Current;

            var user = context.User.FromCurrent<T>();

            if (condition(user)) return;

            context.Response.StatusCode = 403;
            context.Response.End();
        }
    }
}