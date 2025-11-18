using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CorarlERP.BatchNos;
using CorarlERP.DeliverySchedules;
using CorarlERP.Items;
using CorarlERP.KitchenOrders;
using CorarlERP.Lots;
using CorarlERP.PhysicalCount;
using CorarlERP.Productions;
using CorarlERP.SaleOrders;
using CorarlERP.TransferOrders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.ItemIssues
{
    [Table("CarlErpItemIssueItems")]
    public class ItemIssueItem : AuditedEntity<Guid>, IMayHaveTenant
    {
        public Guid ItemIssueId { get; private set; }
        public ItemIssue ItemIssue { get; private set; }
        public void SetItemIssue(Guid itemIssueId) => ItemIssueId = itemIssueId; 

        public Guid? SaleOrderItemId { get; private set; }
        public SaleOrderItem SaleOrderItem { get; private set; }

        public Guid? TransferOrderItemId { get; private set; }
        public TransferOrderItem TransferOrderItem { get; private set; }
        public Guid? RawMaterialItemId { get; private set; }
        public RawMaterialItems RawMaterialItem { get; private set; }

        public KitchenOrderItemAndBOMItem KitchenOrderItemAndBOMItem { get; private set; }
        public Guid? KitchenOrderItemAndBOMItemId { get; private set; }
        public Guid? PhysicalCountItemId { get; private set; }
        public PhysicalCountItem PhysicalCountItem { get; private set; }
        public void SetPhysicalCountItem(Guid? physicalCountItemId) => PhysicalCountItemId = physicalCountItemId;

        public Guid ItemId { get; private set; }
        public Item Item { get; private set; }

        public string Description { get; private set; }

        public decimal Qty { get; private set; }

        public decimal UnitCost { get; private set; }

        public decimal DiscountRate { get; private set; }

        public decimal Total { get; private set; }

        public int? TenantId { get; set; }

        public Lot Lot { get; private set; }
        public long? LotId { get; private set; }

        //add delivery schedule item
        public Guid? DeliverySchedulItemId { get; private set; }
        public DeliveryScheduleItem DeliveryScheduleItem { get; private set; }

        public void SetDeliverySchedulItem(Guid? deliverySchedulItemId) => this.DeliverySchedulItemId = deliverySchedulItemId;
        private static ItemIssueItem Create(
            int? tenantId, long? creatorUserId,
            Guid itemId, string description,
            decimal qty, decimal unitCost,
            decimal discountRate, decimal total)
        {
            return new ItemIssueItem
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
            };
        }
        public void UpdateLot (long? lotId)
        {
            LotId = lotId;
        }
        public void SetKitchenOrderItemAndBOMItemId(Guid? kitchenOrderItemAndBOMItemId)
        {
            this.KitchenOrderItemAndBOMItemId = kitchenOrderItemAndBOMItemId;
        }
        //use for create ItemIssueItem in Update ItemReceipt  (when we have ItemIssueId)
        public static ItemIssueItem Create(
            int? tenantId,
            long? creatorUserId,
            Guid itemIssuseId,
            Guid itemId,
            string description,
            decimal qty,
            decimal unitCost,
            decimal discountRate,
            decimal total)
        {
            var result = Create(tenantId, creatorUserId,
                itemId, description, qty, unitCost, discountRate, total);
            result.ItemIssueId = itemIssuseId;
            return result;
        }

        //use for create Item Issue Item in Update item Receipt in Create new item Issue(when this ItemIssueItem does not have Id yet)       
        public static ItemIssueItem Create(
            int? tenantId,
            long? creatorUserId,
            ItemIssue itemIssue,
            Guid itemId,
            string description,
            decimal qty,
            decimal unitCost,
            decimal discountRate,
            decimal total)
        {
            var result = Create(tenantId, creatorUserId, itemId, description, qty, unitCost, discountRate, total);
            result.ItemIssue = itemIssue;
            return result;

        }

        public void IncreaseItemIssueItemQty(decimal itemIssueItemQtyToIncrease)
        {
            Qty += itemIssueItemQtyToIncrease;
        }
        public void UpdateSaleOrderItemId(Guid? saleOrderItemId)
        {
            SaleOrderItemId = saleOrderItemId;
        }
        public void UpdateDeliveryScheduleItemId(Guid? deliveryScheduleItemId)
        {
            this.DeliverySchedulItemId = deliveryScheduleItemId;
        }

        public void UpdateTransferOrderItemId(Guid? transferOrderItemId)
        {
            TransferOrderItemId = transferOrderItemId;
        }
        public void UpdateRawmatailItemId(Guid? id)
        {
            RawMaterialItemId = id;
        }

        public void UpdateQtyKitchenOrder(decimal qty, decimal total)
        {
            this.Qty = qty;
            this.Total = total;       
        }

        // ItemIssue cannot just change ItemIssue
        public void Update(long lastModifiedUserId,
            Guid itemId,
            string description,
            decimal qty, 
            decimal unitCost,
            decimal discountRate,
            decimal total)
        {
            LastModifierUserId = lastModifiedUserId;
            Description = description;
            Total = total;
            Qty = qty;
            DiscountRate = discountRate;
            ItemId = itemId;
            UnitCost = unitCost;
        }

        public void UpdateQty(decimal qty)
        {
            Qty = qty;
        }

        // ItemIssue Item for when do auto calculate
        public void UpdateUnitCost(
            decimal unitCost,
            decimal total)
        {
            Total = total;
            UnitCost = unitCost;
        }

        public void UpdateItemIssueItem(decimal unitCost, decimal discountRate, decimal total)
        {
            Total = total;
            UnitCost = unitCost;
            DiscountRate = discountRate;
        }


        //#region Overriden Equals
        //public override bool Equals(object obj)
        //{
        //    return this.Equals(obj as ItemIssueItem);
        //}

        //public bool Equals(ItemIssueItem p)
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

        //public bool Equals(ItemIssueItem x, ItemIssueItem y)
        //{
        //    return x.Equals(y);
        //}

        //public override int GetHashCode()
        //{
        //    return Id.GetHashCode();
        //}

        //public static bool operator ==(ItemIssueItem lhs, ItemIssueItem rhs)
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

        //public static bool operator !=(ItemIssueItem lhs, ItemIssueItem rhs)
        //{
        //    return !(lhs == rhs);
        //}
        //#endregion
    }
}
