using System.Configuration;

namespace Frankstein.Common.Configuration
{
    public class HttpModulesElement : ConfigurationElement
    {
        [ConfigurationProperty("trace")]
        public TraceElement Trace
        {
            get { return (TraceElement)this["trace"]; }
            set { this["trace"] = value; }
        }

        [ConfigurationProperty("customerror")]
        public CustomErrorElement CustomError
        {
            get { return (CustomErrorElement)this["customerror"]; }
            set { this["customerror"] = value; }
        }

        [ConfigurationProperty("whitespace")]
        public WhiteSpaceElement WhiteSpace
        {
            get { return (WhiteSpaceElement)this["whitespace"]; }
            set { this["whitespace"] = value; }
        }

        [ConfigurationProperty("pathrewriter")]
        public PathRewriter PathRewriter
        {
            get { return (PathRewriter)this["pathrewriter"]; }
            set { this["pathrewriter"] = value; }
        }
    }
}