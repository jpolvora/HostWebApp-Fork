using System.ComponentModel.DataAnnotations;
using Frankstein.EntityFramework;

namespace Frankstein.DbFileSystem
{
    public class DbModule : AuditableEntity<int>
    {
        [StringLength(150)]
        public string Name { get; set; }
        public byte[] Zip { get; set; }
        public byte[] CompiledAssembly { get; set; }
    }
}