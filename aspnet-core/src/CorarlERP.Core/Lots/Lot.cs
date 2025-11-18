using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Locations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Lots
{
    [Table("CarlErpLots")]
    public class Lot : AuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxLotNameLength = 512;
        public int? TenantId { get; set; }

        [Required]
        [MaxLength(MaxLotNameLength)]
        public string LotName { get; private set; }
        public long  LocationId { get; private set; }
        public Location Location { get; private set; }

        [Required]
        public bool IsActive { get; private set; }

        public static Lot Create(int? tenantId, long creatorUserId, string lotName, long locationId)
        {
            return new Lot()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                IsActive = true,
                LocationId = locationId,
                LotName = lotName,
          
            };
        }
        public void Enable(bool isEnable)
        {
            IsActive = isEnable;
        }
        public void Update(long lastModifiedUserId, string lotName, long locationId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            LotName = lotName;
            LocationId = locationId;         
        }
    }
}
