using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Locations;

namespace CorarlERP.PurchasePrices
{
    [Table("CarlErpPurchasePrices")]
    public class PurchasePrice : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public Location Location { get; private set; }
        public long? LocationId { get; private set; }

        public bool SpecificVendor { get; private set; }
        public ICollection<PurchasePriceItem> PurchasePriceItems { get; private set; }

        public static PurchasePrice Create(int? tenantId, long creatorUserId, long? locationId, bool speificVendor)
        {
            return new PurchasePrice()
            {
                Id = new Guid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                LocationId = locationId,
                SpecificVendor = speificVendor
            };
        }


        public void Update(long lastModifiedUserId, long? locationId, bool speificVendor)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            LocationId = locationId;
            SpecificVendor = speificVendor;
        }

    }
}
