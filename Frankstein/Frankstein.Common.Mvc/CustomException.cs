using System;

namespace Frankstein.Common.Mvc
{
    public class CustomException : Exception
    {
        public CustomException(string msg)
            : base(msg)
        {

        }
    }
}
