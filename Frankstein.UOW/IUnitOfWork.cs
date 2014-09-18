using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UOF
{
    public enum UnitOfWorkStatus
    {
        None,
        Success,
        Fail
    }

    public interface IUnitOfWork : IDisposable
    {
        UnitOfWorkStatus Status { get; }
        bool Commit();
        void RollBack();
    }

    public class UnitOfWorkBase : IUnitOfWork
    {
        public void Dispose()
        {
            RollBack();
        }

        public Exception Exception { get; private set; }

        public UnitOfWorkStatus Status { get; private set; }

        protected Queue<Action<Action>> Actions = new Queue<Action<Action>>();

        public bool Commit()
        {
            if (Status == UnitOfWorkStatus.None)
            {
                try
                {
                    CommitWork();
                    Status = UnitOfWorkStatus.Success;
                    return true;
                }
                catch (Exception ex)
                {
                    Exception = ex;
                    Status = UnitOfWorkStatus.Fail;
                    return false;
                }
            }

            return Status == UnitOfWorkStatus.Success;
        }

        public virtual void CommitWork()
        {
            Action<Action> action = null;
            while ((action = Actions.Dequeue()) != null)
            {
                action.Invoke(() => { });
            }
        }

        public void RollBack()
        {
            if (Status == UnitOfWorkStatus.Fail)
            {

            }
        }

        public virtual void RollBackWork()
        {

        }
    }

    public class DbContextUnitOfWork : UnitOfWorkBase
    {
        public DbContextUnitOfWork(Func<int> saveChangesDelegate)
        {

        }
    }
}
