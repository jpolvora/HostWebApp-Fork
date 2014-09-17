using System;
using System.Collections.Generic;

namespace Frankstein.DOP
{
    public class AspectException : Exception
    {
        public List<Exception> Exceptions { get; private set; }

        public AspectException(List<Exception> exceptions)
        {
            Exceptions = exceptions;
        }
    }
}