using System;

namespace Frankstein.Common
{
    public class CustomException : Exception
    {
        public CustomException(string msg)
            : base(msg)
        {

        }
    }
}
