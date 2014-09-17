using System.Transactions;
using System.Web;

namespace Frankstein.HttpModules
{
    public static class TransactionScopeExtensions
    {
        public static TransactionScope FromCurrentRequest(this HttpContext context)
        {
            if (context == null)
                return null;

            var scope = TransactionScopeHttpModule.GetTransactionScope(context);

            return scope;
        }

        public static TransactionScope FromCurrentRequest(this HttpContextBase context)
        {
            if (context == null)
                return null;

            var scope = TransactionScopeHttpModule.GetTransactionScope(context);

            return scope;
        }
    }
}