using System.Threading.Tasks;

namespace Frankstein.Common.Mvc.Scheduling
{
    public interface IJob
    {
        void Register();
        void Unregister();
        Task Execute();
    }
}