using System;

namespace Frankstein.Common.Mvc.Scheduling
{
    public class RecurringActionJob : RecurringJobBase
    {
        private readonly Action _action;

        public RecurringActionJob(string name, int interval, Action action, bool continueOnError)
            : base(name, interval, continueOnError)
        {
            _action = action;
        }

        public override void Execute()
        {
            _action();
        }
    }
}