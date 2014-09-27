namespace Frankstein.Common.Mvc.Scheduling
{
    public interface IJob
    {
        void Register();
        void Unregister();
        void Execute();
    }
}