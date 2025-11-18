using Abp.Timing;
using CorarlERP.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.InventoryCalculationItems
{

    [Table("CarlErpInventoryCalculationItems")]
    public class InventoryCalculationItem: BaseAuditedEntity<Guid>
    {
        public Guid ItemId { get; private set; }
        public virtual Item Item { get; private set; }
        public  DateTime Date { get; private set; }

        public static InventoryCalculationItem Create(int tenantId, long userId, DateTime date, Guid itemId)
        {
            return new InventoryCalculationItem
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                Date = date,
                ItemId = itemId
            };
        }

        public void Update(long userId, DateTime date, Guid itemId)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            Date = date;
            ItemId = itemId;
        }

    }
}
