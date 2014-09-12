using System;

namespace Frankstein.DbFileSystem.EF
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