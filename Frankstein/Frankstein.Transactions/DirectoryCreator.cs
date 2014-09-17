using System.IO;
using System.Transactions;

namespace Frankstein.Transactions
{
    //todo: implement as explicit
    public class DirectoryCreator : IEnlistmentNotification
    {
        public static void Create(string directoryName)
        {
            Transaction.Current.EnlistVolatile(new DirectoryCreator(directoryName), EnlistmentOptions.None);
        }

        private readonly string _directoryName;
        private bool _isCommitSucceed = false;

        private DirectoryCreator(string directoryName)
        {
            _directoryName = directoryName;

        }
        void IEnlistmentNotification.Commit(Enlistment enlistment)
        {
            Directory.CreateDirectory(_directoryName);
            _isCommitSucceed = true;
            enlistment.Done();
        }

        void IEnlistmentNotification.InDoubt(Enlistment enlistment)
        {
            enlistment.Done();
        }

        void IEnlistmentNotification.Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }

        void IEnlistmentNotification.Rollback(Enlistment enlistment)
        {
            if (_isCommitSucceed)
                Directory.Delete(_directoryName);
            enlistment.Done();
        }
    }
}