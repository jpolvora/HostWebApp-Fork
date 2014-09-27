using Frankstein.EntityFramework;

namespace Frankstein.DbFileSystem
{
    public class DbModule : AuditableEntity<int>
    {
        public string Name { get; set; }
        public byte[] Zip { get; set; }
    }
}