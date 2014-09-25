using System;
using System.Threading.Tasks;

namespace Frankstein.Common.Mvc.Scheduling
{
    public class ForeverActionJob : ForeverJobBase
    {
        private readonly Func<Task> _action;

        public ForeverActionJob(string name, int interval, Func<Task> action)
            : base(name, interval)
        {
            _action = action;
        }

        public async override Task Execute()
        {
            await base.Execute();
            await _action();
        }
    }
}