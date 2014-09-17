using System.Configuration;

namespace Frankstein.Common.Configuration
{
    public class TransactionScopeElement : BooleanElementBase
    {
        [ConfigurationProperty("timeout", DefaultValue = 30)]
        public int TimeOut
        {
            get { return (int)this["timeout"]; }
            set { this["timeout"] = value; }
        }
    }
}