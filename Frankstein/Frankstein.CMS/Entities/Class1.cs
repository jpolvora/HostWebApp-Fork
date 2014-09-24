using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Frankstein.EntityFramework;

namespace Frankstein.CMS.Entities
{
    public enum ResourceKind
    {
        Image,
        Text,
        Pdf,
        Doc,
        Video,
        Sound
    }

    [Table("MediaResources")]
    public class CmsResource : AuditableEntity<long>
    {
        public ResourceKind Type { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [Required, MaxLength]
        public string Description { get; set; }
        [Required, StringLength(512)]
        public string Url { get; set; }
    }

    [Table("CmsHits")]
    public class CmsHit : AuditableEntity<long>
    {
        public string Ip { get; set; }
        public string HttpMethod { get; set; }
        public string RawUrl { get; set; }
        public string Referer { get; set; }
        public int StatusCode { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string Handler { get; set; }
        public TimeSpan ResponseTime { get; set; }
    }

    public class CmsModule : AuditableEntity<int>
    {
        public string ZipFileName { get; set; }
        public byte[] ZipArchive { get; set; }
        public bool Compiled { get; set; }
        public byte[] Assembly { get; set; }
    }
}
