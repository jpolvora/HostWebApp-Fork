using System;

namespace Frankstein.EntityFramework
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DecimalMappingAttribute : Attribute
    {
        public readonly byte Precision;
        public readonly byte Scale;

        public DecimalMappingAttribute(byte precision, byte scale)
        {
            Precision = precision;
            Scale = scale;

        }
    }
}