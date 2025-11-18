using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CorarlERP.BatchNos;
using CorarlERP.ItemReceipts;
using CorarlERP.Items;
using CorarlERP.Lots;
using CorarlERP.PurchaseOrders;
using CorarlERP.Taxes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorarlERP.Bills
{
    [Table("CarlErpBillItems")]
    public class BillItem : AuditedEntity<Guid>, IMayHaveTenant
    {
        public Guid BillId { get; private set; }
        public Bill Bill { get; private set; }

        public Guid? ItemReceiptItemId { get; private set; }
        public ItemReceiptItem ItemReceiptItem { get; private set; }

        public Guid? OrderItemId { get; private set; }
        public PurchaseOrderItem OrderItem { get; private set; }

        public Guid? ItemId { get; private set; }
        public Item Item { get; private set; }

        //public Guid ItemJournalId { get; private set; }
        //public Journal Journal { get; private set; }

        public long TaxId { get; private set; }
        public Tax Tax { get; private set; }
        
        public string  Description { get; private set; }

        public decimal Qty { get; private set; }

        public decimal UnitCost { get; private set; }

        public decimal MultiCurrencyUnitCost { get; private set; }
        public decimal MultiCurrencyTotal { get; private set; }

        public decimal DiscountRate { get; private set; }

        public decimal Total { get; private set; }
        public void SetTotal(decimal total) { Total = total; }
        
        public int? TenantId { get; set; }

        public bool IsItemReceipt { get; private set; }

        public Lot Lot { get; private set; }
        public long? LotId { get; private set; }

        public Guid? ParentId { get; private set; }
        public void SetParent(Guid? billItemId) => ParentId = billItemId;
        public void SetBill(Guid billId) => BillId = billId;

        private static BillItem Create(int? tenantId, long? creatorUserId, long taxId, Guid? itemId,
                string description, decimal qty, decimal unitCost, decimal discountRate, decimal total,
                decimal multiCurrencyUntiCost, decimal multiCurrencyTotal)
        {
            return new BillItem
            {
                Id = Guid.NewGuid(),
                IsItemReceipt = false,
                CreatorUserId = creatorUserId,
                TenantId = tenantId,
                Description = description,
                Total = total,
                Qty = qty,
                DiscountRate = discountRate,
                ItemId = itemId,
                UnitCost = unitCost,
                TaxId = taxId,
                MultiCurrencyTotal= multiCurrencyTotal,
                MultiCurrencyUnitCost = multiCurrencyUntiCost,
            };
        }

        public void UpdateLot (long? lotId)
        {
            LotId = lotId;
        }
        // update isRececipt 

           public void UpdateIsReceipt (bool isReceipt)
        {
            IsItemReceipt = isReceipt; 
        }

        //use for create BillItem in Update ItemReceipt  (when we have BillId)
        public static BillItem Create(
            int? tenantId,
            long? creatorUserId,
            Guid billId,
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
            var result = Create(tenantId, creatorUserId,
                taxId, itemId, description, qty, unitCost, discountRate, total,multiCurrencyUnitCost,multiCurrencyTotal);
            result.BillId = billId;
            return result;
        }
        //use for create bill Item in Update bill in Create new bill(when this billItem does not have Id yet)       
        public static BillItem Create(
            int? tenantId,
            long? creatorUserId,
            Bill bill,
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
            var result = Create(tenantId, creatorUserId, taxId, itemId, description, qty, unitCost, discountRate, total,multiCurrencyUnitCost,multiCurrencyTotal);
            result.Bill = bill;
            return result;

        }

        public void UpdateReceiptItemId(Guid? receiptItemId)
        {
            this.ItemReceiptItemId = receiptItemId;
        }

        public void UpdateOrderItemId(Guid? orderItemId)
        {
            this.OrderItemId = orderItemId;
        }                  

        public void IncreaseBillItemQty(decimal billQtyToIncrease)
        {
            Qty += billQtyToIncrease;
        }

        // bill cannot just change bill
        public void Update(long lastModifiedUserId,
            long taxId, Guid? itemId,
            string description, decimal qty, decimal unitCost, decimal discountRate, decimal total, decimal multiCurrencyUnitCost,
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
