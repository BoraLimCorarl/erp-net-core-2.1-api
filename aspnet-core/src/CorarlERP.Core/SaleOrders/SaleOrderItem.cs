using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CorarlERP.ChartOfAccounts;
using CorarlERP.DeliverySchedules;
using CorarlERP.Invoices;
using CorarlERP.ItemIssues;
using CorarlERP.Items;
using CorarlERP.Taxes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.SaleOrders
{
    [Table("CarlErpSaleOrderItems")]
    public class SaleOrderItem : AuditedEntity<Guid>, IMayHaveTenant
    {
        [Required]
        public Guid ItemId { get; private set; }
        public Item Item { get; private set; }

        public Guid SaleOrderId { get; private set; }
        public SaleOrder SaleOrder { get; private set; }
       
        public long TaxId { get; private set; }
        public Tax Tax { get; private set; }
        public decimal TaxRate { get; private set; }

        public string Description { get; private set; }
        public decimal Qty { get; private set; }
        public decimal UnitCost { get; private set; }
        public decimal DiscountRate { get; private set; }
        public decimal Total { get; private set; }
        public int? TenantId { get; set; }
        
        public decimal MultiCurrencyUnitCost { get; private set; }
        public decimal MultiCurrencyTotal { get; private  set; }

        public void SetTotal(decimal total)
        {
            Total = total;
        }
        public virtual ICollection<InvoiceItem> InvoiceItems { get; private set; }
        public virtual ICollection<ItemIssueItem> ItemIssueItems { get; private set; }
       // public  ICollection<DeliveryScheduleItem> DeliveryScheduleItems { get; private set; }

        private static SaleOrderItem Create(
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
            decimal multiCurrencyTotal)
        {
            return new SaleOrderItem
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
            };
        }

        public static SaleOrderItem Create(
            int? tenantId, 
            long? creatorUserId, 
            Guid itemId, 
            Guid saleOrderId,
            long taxId, 
            decimal taxRate, 
            string description, 
            decimal unit, 
            decimal unitCost, 
            decimal discount, 
            decimal total,
            decimal multiCurrencyUnitCost,
            decimal multiCurrencyTotal)
        {
            var result = Create(
                tenantId, 
                creatorUserId, 
                itemId, 
                taxId, 
                taxRate, 
                description, 
                unit, 
                unitCost, 
                discount, 
                total,
                multiCurrencyUnitCost,
                multiCurrencyTotal);
            result.SaleOrderId = saleOrderId;
            return result;
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
            decimal multiCurrencyTotal)
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
            //AccountId = accountId;
        }
        
    }
}
