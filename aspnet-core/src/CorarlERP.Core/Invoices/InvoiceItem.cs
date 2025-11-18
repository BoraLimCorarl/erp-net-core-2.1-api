using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CorarlERP.BatchNos;
using CorarlERP.DeliverySchedules;
using CorarlERP.ItemIssues;
using CorarlERP.Items;
using CorarlERP.Lots;
using CorarlERP.SaleOrders;
using CorarlERP.Taxes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Invoices
{
    [Table("CarlErpInvoiceItems")]
    public class InvoiceItem : AuditedEntity<Guid>, IMayHaveTenant
    {
        public Guid InvoiceId { get; private set; }
        public Invoice Invoice { get; private set; }
        public void SetInvoice(Guid invoiceId) => InvoiceId = invoiceId;

        public Guid? ItemIssueItemId { get; private set; }
        public ItemIssueItem ItemIssueItem { get; private set; }

        public Guid? OrderItemId { get; private set; }
        public SaleOrderItem SaleOrderItem { get; private set; }


        //add delivery schedule item
        public Guid? DeliverySchedulItemId { get; private set; }
        public DeliveryScheduleItem DeliveryScheduleItem { get; private set; }

        public Guid? ItemId { get; private set; }
        public Item Item { get; private set; }
        
        public long TaxId { get; private set; }
        public Tax Tax { get; private set; }

        public string Description { get; private set; }

        public decimal Qty { get; private set; }

        public decimal UnitCost { get; private set; }

        public decimal DiscountRate { get; private set; }

        public decimal Total { get; private set; }

        public int? TenantId { get; set; }

        public decimal MultiCurrencyUnitCost { get; private set; }
        public decimal MultiCurrencyTotal { get; private set; }

        public bool IsItemIssue { get; private set; }

        public Lot Lot { get; private set; }
        public long? LotId { get; private set; }

        public void SetUnitCost(decimal cost) { UnitCost = cost; }
        public void SetTotal(decimal total) { Total = total; }

        public Guid? ParentId { get; private set; }
        public void SetParent(Guid? billItemId) => ParentId = billItemId;

        private static InvoiceItem Create(int? tenantId, long? creatorUserId, long taxId, Guid? itemId, string description, 
            decimal qty, decimal unitCost, decimal discountRate, decimal total, decimal multiCurrencyUntiCost, decimal multiCurrencyTotal)
        {
            return new InvoiceItem
            {
                Id = Guid.NewGuid(),
                IsItemIssue = false,
                CreatorUserId = creatorUserId,
                TenantId = tenantId,
                Description = description,
                Total = total,
                Qty = qty,
                DiscountRate = discountRate,
                ItemId = itemId,
                UnitCost = unitCost,
                TaxId = taxId,
                MultiCurrencyTotal = multiCurrencyTotal,
                MultiCurrencyUnitCost = multiCurrencyUntiCost,
            };
        }

        // update isRececipt 

        public void UpdateLot(long? lotId)
        {
            LotId = lotId;
        }

        public void UpdateIsItemIssue(bool isItemIssue)
        {
            IsItemIssue = isItemIssue;
        }

        //use for create BillItem in Update ItemReceipt  (when we have BillId)
        public static InvoiceItem Create(
            int? tenantId,
            long? creatorUserId,
            Guid invoiceId,
            long taxId,
            Guid? itemId,
            string description,
            decimal qty,
            decimal unitCost,
            decimal discountRate,
            decimal total,
            decimal multiCurrencyUnitCost,
            decimal multiCurrencyTotal)
        {
            var result = Create(
                tenantId,
                creatorUserId,
                taxId, 
                itemId, 
                description, 
                qty, 
                unitCost, 
                discountRate, 
                total,
                multiCurrencyUnitCost,
                multiCurrencyTotal
            );
            result.InvoiceId = invoiceId;
            return result;
        }
        //use for create invoice Item in Update invoice in Create new invoice(when this invoiceItem does not have Id yet)       
        public static InvoiceItem Create(
            int? tenantId,
            long? creatorUserId,
            Invoice invoice,
            long taxId,
            Guid? itemId,
            string description,
            decimal qty,
            decimal unitCost,
            decimal discountRate,
            decimal total,
            decimal multiCurrencyUnitCost,
            decimal multiCurrencyTotal)
        {
            var result = Create(tenantId, creatorUserId, taxId, itemId, description, qty, unitCost, discountRate, total, multiCurrencyUnitCost, multiCurrencyTotal);
            result.Invoice = invoice;
            return result;
        }

        public void UpdateIssueItemId(Guid? issueItemId)
        {
            this.ItemIssueItemId = issueItemId;
        }

        public void UpdateOrderItemId(Guid? orderItemId)
        {
            this.OrderItemId = orderItemId;
        }
        public void SetDeliverySchedulItem(Guid? deliverySchedulItemId) => this.DeliverySchedulItemId = deliverySchedulItemId;

        public void IncreaseItemQty(decimal qty)
        {
            Qty += qty;
        }
        
        public void Update(long lastModifiedUserId,
            long taxId, Guid? itemId,
            string description, decimal qty, decimal unitCost, decimal discountRate, decimal total,
            decimal multiCurrencyUnitCost,
            decimal multiCurrencyTotal)
        {
            LastModifierUserId = lastModifiedUserId;
            Description = description;
            Total = total;
            Qty = qty;
            DiscountRate = discountRate;
            ItemId = itemId;
            UnitCost = unitCost;
            TaxId = taxId;
            MultiCurrencyUnitCost = multiCurrencyUnitCost;
            MultiCurrencyTotal = multiCurrencyTotal;
        }
    }
}
