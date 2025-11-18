using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CorarlERP.Bills;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ItemReceipts;
using CorarlERP.Items;
using CorarlERP.Taxes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.PurchaseOrders
{
    [Table("CarlErpPurchaseOrderItems")]
    public class PurchaseOrderItem :AuditedEntity<Guid>, IMayHaveTenant
    {
        [Required]
        public Guid ItemId { get; private set; }
        public Item Item { get; private set; }

        public Guid PurchaseOrderId { get; private set; }
        public PurchaseOrder PurchaseOrder { get; private set; }

        //public Guid PurchaseAccountId { get; private set; }
        //public ChartOfAccount PurchaseAccount { get; private set; }

        public long TaxId { get; private set; }
        public Tax Tax { get; private set; }
        public decimal TaxRate { get; private set; }

        public string Description { get; private set; }
        public decimal Unit { get; private set; }
        public decimal UnitCost { get; private set; }
        public decimal MultiCurrencyUnitCost { get; private set; }
        public decimal DiscountRate { get; private set; }
        public decimal Total { get; private set; }

        public decimal MultiCurrencyTotal { get; private  set; }
     
        public int? TenantId { get; set; }

        public void SetTotal(decimal total)
        {
            Total = total;
        }

        public virtual ICollection<BillItem> BillItems { get; private set; }
        public virtual ICollection<ItemReceiptItem> ItemReceiptItems { get; private set; }

        private static PurchaseOrderItem Create(int? tenantId, long? creatorUserId, Guid itemId, long taxId,decimal taxRate,string description,decimal unit ,decimal unitCost,decimal discount,decimal total,decimal multiCurrencyTotal,decimal multiCurrencyUnitCost)
        {
            return new PurchaseOrderItem
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                TenantId = tenantId,
                ItemId = itemId,                            
                TaxId = taxId,
                TaxRate= taxRate,
                Description = description,
                Unit= unit,
                UnitCost = unitCost,
                DiscountRate= discount,
                Total =total,  
                MultiCurrencyTotal = multiCurrencyTotal,
                MultiCurrencyUnitCost = multiCurrencyUnitCost,
               
            };
        }
      
        public static PurchaseOrderItem Create(int? tenantId, long? creatorUserId, Guid itemId, Guid purchaseOrderId, 
               long taxId,decimal taxRate, string description, decimal unit, decimal unitCost, decimal discount, decimal total, decimal multiCurrencyTotal, decimal multiCurrencyUnitCost)
        {
            var result = Create(tenantId, creatorUserId, itemId, taxId,taxRate, description, unit, unitCost, discount, total, multiCurrencyTotal, multiCurrencyUnitCost);
            result.PurchaseOrderId = purchaseOrderId;
            return result;
        }

        public void Update(long lastModifiedUserId,Guid itemId, long taxId,decimal taxRate, string description, decimal unit, decimal unitCost, decimal discount, decimal total, decimal multiCurrencyTotal, decimal multiCurrencyUnitCost)
        {
            LastModifierUserId = lastModifiedUserId;
            ItemId = itemId;                     
            TaxId = taxId;
            Description = description;
            Unit = unit;
            UnitCost = unitCost;
            DiscountRate = discount;
            Total = total;
            TaxRate = TaxRate;
            MultiCurrencyTotal = multiCurrencyTotal;
            MultiCurrencyUnitCost = multiCurrencyUnitCost;
        }        
    }
}
