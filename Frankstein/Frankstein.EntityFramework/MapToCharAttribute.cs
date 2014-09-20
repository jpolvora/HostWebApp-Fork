using System;

namespace Frankstein.EntityFramework
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MapToCharAttribute : Attribute
    {
        public readonly int FixedLength;

        public MapToCharAttribute(int fixedLength)
        {
            FixedLength = fixedLength;
        }
    }
}