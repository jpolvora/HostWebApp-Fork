using System.Threading.Tasks;

namespace Frankstein.Common.Mvc.Jobs
{
    public interface IJob
    {
        void Start();
        Task Execute();
    }
}