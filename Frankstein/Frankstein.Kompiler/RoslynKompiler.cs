using System;
using System.Collections.Generic;

namespace Frankstein.Kompiler
{
    public class RoslynKompiler : IKompiler
    {
        public string CompileFromSource(Dictionary<string, string> files, out byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public string CompileFromFolder(string folder, out byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public string CompileString(string text, out byte[] buffer)
        {
            throw new NotImplementedException();
        }
    }
}