using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CorarlERP.BatchNos;
using CorarlERP.Bills;
using CorarlERP.Items;
using CorarlERP.Journals;
using CorarlERP.Lots;
using CorarlERP.PhysicalCount;
using CorarlERP.Productions;
using CorarlERP.PurchaseOrders;
using CorarlERP.Taxes;
using CorarlERP.TransferOrders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.ItemReceipts
{
    [Table("CarlErpItemReceiptItems")]
    public class ItemReceiptItem : AuditedEntity<Guid>, IMayHaveTenant
    {
        public Guid ItemReceiptId { get; private set; }
        public ItemReceipt ItemReceipt { get; private set; }

        public Guid? OrderItemId { get; private set; }
        public PurchaseOrderItem OrderItem { get; private set; }

        //public Guid? BillItemId { get; private set; }
        //public BillItem BillItem { get; private set; }

        public Guid  ItemId { get; private set; }
        public Item Item { get; private set; }

        //public long TaxId { get; private set; }
        //public Tax Tax { get; private set; }

        public string  Description { get; private set; }

        public Guid? TransferOrderItemId { get; private set; }
        public TransferOrderItem TransferOrderItem { get; private set; }


        public Guid? FinishItemId { get; private set; }
        public FinishItems FinishItems { get; private set; }

        public Guid? PhysicalCountItemId { get; private set; }
        public PhysicalCountItem PhysicalCountItem { get; private set; }
        public void SetPhysicalCountItem(Guid? physicalCountItemId) => PhysicalCountItemId = physicalCountItemId;

        public decimal Qty { get; private set; }

        public decimal UnitCost { get; private set; }

        public decimal DiscountRate { get; private set; }

        public decimal Total { get; private set; }

        public int? TenantId { get ; set; }

        public long? LotId { get; private set; }
        public Lot Lot { get; private set; }
        public void SetItemReceipt(Guid itemReceiptId) => ItemReceiptId = itemReceiptId;

        private static ItemReceiptItem Create(int? tenantId, long? creatorUserId, Guid itemId, string description, decimal qty, decimal unitCost, decimal discountRate, decimal total)
        {
            return new ItemReceiptItem
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                TenantId = tenantId,
                Description = description,
                Total = total,
                Qty = qty,
                DiscountRate = discountRate,
                ItemId = itemId,
                UnitCost = unitCost,
               // TaxId = taxId
            };
        }
        public void UpdateLot (long? lot)
        {
            LotId = lot;
        }
        //use for create ItemReceiptItem in Update ItemReceipt  (when we have ItemReceiptId)
        public static ItemReceiptItem Create(
            int? tenantId,
            long? creatorUserId,
            Guid itemReceiptId,           
            Guid itemId, 
            string description, 
            decimal qty, 
            decimal unitCost, 
            decimal discountRate, 
            decimal total)
        {
            var result = Create(tenantId, creatorUserId,
                itemId, description, qty, unitCost, discountRate, total);
            result.ItemReceiptId = itemReceiptId;
            return result;
        }
        //use for create Item Receipt Item in Update item Receipt in Create new item Receipt(when this ItemReceiptItem does not have Id yet)       
        public static ItemReceiptItem Create(
            int? tenantId, 
            long? creatorUserId,
            ItemReceipt itemReceipt,         
            Guid itemId, 
            string description, 
            decimal qty, 
            decimal unitCost, 
            decimal discountRate, 
            decimal total)
        {
            var result = Create(tenantId, creatorUserId, itemId, description, qty, unitCost, discountRate, total);           
            result.ItemReceipt = itemReceipt;
            return result;
            
        }

        //public void UpdateBillItemId(Guid billItemId)
        //{
        //    this.BillItemId = billItemId;
        //}
        public void IncreaseItemReceiptItemQty(decimal itemReceiptItemQtyToIncrease)
        {
            Qty += itemReceiptItemQtyToIncrease;
        }
        public void UpdateOrderItemId (Guid? orderItemId)
        {
            this.OrderItemId = orderItemId;
        }

        public void UpdateTransferOrderItemId(Guid? id) {
            TransferOrderItemId = id;
        }

        public void UpdateFinishItemId (Guid? id)
        {
            FinishItemId = id;
        }

        // ItemReceipt cannot just change ItemReceipt
        public void Update(long lastModifiedUserId,
            Guid itemId,
            string description, decimal qty, decimal unitCost, decimal discountRate, decimal total)
        {
            LastModifierUserId = lastModifiedUserId;
            Description = description;
            Total = total;
            Qty = qty;
            DiscountRate = discountRate;
            ItemId = itemId;
            UnitCost = unitCost;           
        }

        public void UpdateItemReceiptItem (decimal unitCost,decimal discountRate, decimal total)
        {
            Total = total;
            UnitCost = unitCost;
            DiscountRate = discountRate;
        }

        public void SetTotal(decimal total) { Total = total; }
        public void UpdateQty (decimal qty)
        {
            this.Qty = qty;
        }

        public void UpdateUnitCost(decimal unitCost, decimal total)
        {
            Total = total;
            UnitCost = unitCost;
        }

        ////overriding equals
        //public override bool Equals(object obj)
        //{
        //    return this.Equals(obj as ItemReceiptItem);
        //}

        //public bool Equals(ItemReceiptItem p)
        //{
        //    // If parameter is null, return false.
        //    if (Object.ReferenceEquals(p, null))
        //    {
        //        return false;
        //    }

        //    // Optimization for a common success case.
        //    if (Object.ReferenceEquals(this, p))
        //    {
        //        return true;
        //    }

        //    // If run-time types are not exactly the same, return false.
        //    if (this.GetType() != p.GetType())
        //    {
        //        return false;
        //    }

        //    // Return true if the fields match.
        //    // Note that the base class is not invoked because it is
        //    // System.Object, which defines Equals as reference equality.
        //    return Id == p.Id;
        //}

        //public bool Equals(ItemReceiptItem x, ItemReceiptItem y)
        //{
        //    return x.Equals(y);
        //}

        //public override int GetHashCode()
        //{
        //    return Id.GetHashCode();
        //}

        //public static bool operator ==(ItemReceiptItem lhs, ItemReceiptItem rhs)
        //{
        //    // Check for null on left side.
        //    if (Object.ReferenceEquals(lhs, null))
        //    {
        //        if (Object.ReferenceEquals(rhs, null))
        //        {
        //            // null == null = true.
        //            return true;
        //        }

        //        // Only the left side is null.
        //        return false;
        //    }

        //    // Equals handles case of null on right side.
        //    return lhs.Equals(rhs);
        //}

        //public static bool operator !=(ItemReceiptItem lhs, ItemReceiptItem rhs)
        //{
        //    return !(lhs == rhs);
        //}
    }
}
