using CorarlERP.Boms;
using CorarlERP.Items;
using CorarlERP.Taxes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.KitchenOrders
{
    [Table("CarlErpKitchenOrderItems")]
    public class KitchenOrderItem : BaseAuditedEntity<Guid>
    {

        [Required]
        public Guid ItemId { get; private set; }
        public Item Item { get; private set; }
        public Guid KitchenOrderId { get; private set; }
        public KitchenOrder KitchenOrder { get; private set; }
        public Guid BOMId { get; private set; }
        public Bom Bom { get; private set; }
        public long TaxId { get; private set; }
        public Tax Tax { get; private set; }
        public decimal TaxRate { get; private set; }
        public string Description { get; private set; }
        public decimal Qty { get; private set; }
        public decimal UnitCost { get; private set; }
        public decimal DiscountRate { get; private set; }
        public decimal Total { get; private set; }
        public decimal MultiCurrencyUnitCost { get; private set; }
        public decimal MultiCurrencyTotal { get; private set; }

        public void SetKitchenOrderId(Guid kitchenOrderId) { KitchenOrderId = kitchenOrderId; }
        public static KitchenOrderItem Create(
           int? tenantId,
           long? creatorUserId,
           Guid itemId,
           long taxId,
           decimal taxRate,
           string description,
           decimal unit,
           decimal unitCost,
           decimal discount,
           decimal total,
           decimal multiCurrencyUnitCost,
           decimal multiCurrencyTotal,
           Guid bomId,
           Guid kitchenOrderId)
        {
            return new KitchenOrderItem
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                TenantId = tenantId,
                ItemId = itemId,
                TaxId = taxId,
                TaxRate = taxRate,
                Description = description,
                Qty = unit,
                UnitCost = unitCost,
                DiscountRate = discount,
                Total = total,
                MultiCurrencyTotal = multiCurrencyTotal,
                MultiCurrencyUnitCost = multiCurrencyUnitCost,
                BOMId =bomId,
                KitchenOrderId = kitchenOrderId,
            };
        }

        public void SetNewQty(decimal qty, decimal total, decimal mTotal)
        {
            Qty = qty;
            Total = this.Total + total;
            MultiCurrencyTotal = this.MultiCurrencyTotal + mTotal;
        }
        public void Update(
           long lastModifiedUserId,
           Guid itemId,
           long taxId,
           decimal taxRate,
           string description,
           decimal unit,
           decimal unitCost,
           decimal discount,
           decimal total,
           decimal multiCurrencyUnitCost,
           decimal multiCurrencyTotal,
           Guid bomId,
           Guid kitchenOrderId)
        {
            LastModifierUserId = lastModifiedUserId;
            ItemId = itemId;
            TaxId = taxId;
            Description = description;
            Qty = unit;
            UnitCost = unitCost;
            DiscountRate = discount;
            Total = total;
            TaxRate = TaxRate;
            MultiCurrencyUnitCost = multiCurrencyUnitCost;
            MultiCurrencyTotal = multiCurrencyTotal;
            BOMId = bomId;
            KitchenOrderId = kitchenOrderId;
            TaxRate = taxRate;
        }

    }
}
