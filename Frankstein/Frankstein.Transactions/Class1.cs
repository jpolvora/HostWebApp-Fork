using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Frankstein.Transactions
{
    /// <summary>
    /// Queue
    /// </summary>
    public class Class1
    {
        public Class1()
        {
            using (var scope = new TransactionScope())
            {
                DirectoryCreator.Create("");

                scope.Complete();
            }
        }
    }
}
