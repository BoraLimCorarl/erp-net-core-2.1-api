using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Formats
{
    [Table("CarlErpFormats")]
    public class Format : AuditedEntity<long>
    {
        public const int MaxNameLength = 50;
        public const int MaxKeyLength = 50;
        public const int MaxWebLength = 50;

        [Required]
        [MaxLength(MaxNameLength)]
        public string Name { get;  set; }
        [MaxLength(MaxKeyLength)]
        public string Key { get; set; }
        [MaxLength(MaxWebLength)]
        public string Web { get; private set; }

        public  static Format Create(long? creatorUserId, string name ,string key, string web)
        {
            return new Format()
            {
                CreatorUserId = creatorUserId,
                Name = name ,
                Key = key,
                Web = web
            };
        }

        public void Update(long? lastModifiedUserId, string name, string key, string web)

        {
            LastModifierUserId = lastModifiedUserId;
            Name = name;
            Key = key;
            Web = web;

        }
    }
}
