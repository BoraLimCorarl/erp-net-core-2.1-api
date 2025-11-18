using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Boms
{
    [Table("CarlErpBOMs")]
    public class Bom : AuditedEntity<Guid>, IMayHaveTenant
    {
        public const int MaxNameLength = 125;
        public const int MaxDescriptionLength = 256;
        public int? TenantId { get; set; }
        public Item Item { get; private set; }
        public Guid ItemId { get; private set; }
        public bool IsDefault { get; private set; }
        [Required]
        [MaxLength(MaxNameLength)]
        public string Name { get; private set; }
        public bool IsActive { get; private set; }

        [MaxLength(MaxDescriptionLength)]
        public string Description { get; private set; }

        public decimal Qty { get; private set; }
        public static Bom Create(int? tenantId,
                               long? creatorUserId,
                               string name,
                               Guid itemId,
                               bool isDefault,
                               string description,
                               decimal qty)
        {
            return new Bom()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                Name = name,
                ItemId = itemId,
                IsActive = true,
                IsDefault = isDefault,
                Description = description,
                Qty = qty
            };
        }
        public void Update(long lastModifiedUserId, string name,Guid itemId,bool isDefault, string description, decimal qty)
        {
            this.LastModifierUserId = lastModifiedUserId;
            this.LastModificationTime = Clock.Now;
            this.Name = name;
            this.IsDefault = isDefault;
            this.ItemId = itemId;
            this.Description = description;
            this.Qty = qty;
        }
        public void Enable()
        {
            this.IsActive = true;
        }
        public void Disable()
        {
            this.IsActive = false;
        }
    }
}
