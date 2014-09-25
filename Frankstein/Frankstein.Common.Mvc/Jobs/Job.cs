using System.Threading.Tasks;

namespace Frankstein.Common.Mvc.Jobs
{
    public class Job : IJob
    {
        public void Register()
        {
            //schedules to a date
        }

        public Task Execute()
        {
            return Task.FromResult(0);
        }
    }
}