using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Frankstein.DbFileSystem
{
    public abstract class AuditableEntity
    {
        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }

        [NotMapped]
        public DateTime LastWriteUtc
        {
            get
            {
                return Modified.HasValue
                    ? Modified.Value.ToUniversalTime()
                    : Created.ToUniversalTime();
            }

        }

        [StringLength(100)]
        public string Modifier { get; set; }

        public bool Status { get; set; }
    }

    public abstract class AuditableEntity<TKey> : AuditableEntity
    {
        [Key]
        public TKey Id { get; set; }
    }
}