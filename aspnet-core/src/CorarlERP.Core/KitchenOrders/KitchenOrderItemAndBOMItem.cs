using Abp.Timing;
using CorarlERP.Boms;
using CorarlERP.Items;
using CorarlERP.Lots;
using CorarlERP.Taxes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.KitchenOrders
{
    [Table("CarlErpKitchenOrderItemAndBOMItems")]
    public class KitchenOrderItemAndBOMItem : BaseAuditedEntity<Guid>
    {
        public decimal Qty { get; private set; }
        public decimal TotalQty { get; private set; }
        public Item Item { get; private set; }
        public Guid ItemId { get; private set; }
        public BomItem BomItem { get; private set; }
        public Guid BomItemId { get; private set; }
        public KitchenOrderItem KitchenOrderItem { get; private set; }
        public Guid KitchenOrderItemId { get; private set; }
        public Lot Lot { get; private set; }
        public long LotId { get; private set; }
        public Tax Tax { get; private set; }
        public long TaxId { get; private set; }
        public decimal TaxRate { get; private set; }

        public static KitchenOrderItemAndBOMItem Create(int? tenantId, long? creatorUserId, Guid itemId, decimal qty, Guid bomItemId, Guid kitchenOrderItemId, long lotId, long taxId, decimal taxRate, decimal totalQty)
        {
            return new KitchenOrderItemAndBOMItem
            {
                BomItemId = bomItemId,
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                Qty = qty,
                ItemId = itemId,
                TotalQty = totalQty,
                KitchenOrderItemId = kitchenOrderItemId,
                LotId = lotId,
                TaxId = taxId,
                TaxRate = taxRate,

            };
        }
        public void Update(long lastModifiedUserId, Guid itemId, decimal qty, Guid bomItemId, Guid kitchenOrderItemId, long lotId, long taxId, decimal taxRate, decimal totalQty)
        {
            LastModifierUserId = lastModifiedUserId;
            Qty = qty;
            ItemId = itemId;
            TotalQty = totalQty;
            KitchenOrderItemId = kitchenOrderItemId;
            LotId = lotId;
            TaxId = taxId;
            TaxRate = taxRate;
        }
        public void SetQty(decimal qty, decimal totalQty)
        {
            this.Qty = qty;
            this.TotalQty = totalQty;
        }
    }
}
