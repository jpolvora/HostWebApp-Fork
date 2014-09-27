using System.Configuration;

namespace Frankstein.Common.Configuration
{
    public class TasksElement : BooleanElementBase
    {
        [ConfigurationProperty("interval", DefaultValue = 300)]
        public int Interval
        {
            get { return (int)this["interval"]; }
            set { this["interval"] = value; }
        }
    }
}