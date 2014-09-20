using System;

namespace Frankstein.EntityFramework
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EmptyStringToNull : Attribute
    {
        
    }
}