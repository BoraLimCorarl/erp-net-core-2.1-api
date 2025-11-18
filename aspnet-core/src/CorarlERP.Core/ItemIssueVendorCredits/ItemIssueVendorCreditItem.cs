using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CorarlERP.BatchNos;
using CorarlERP.ItemReceipts;
using CorarlERP.Items;
using CorarlERP.Lots;
using CorarlERP.Taxes;
using CorarlERP.VendorCredit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.ItemIssueVendorCredits
{
    [Table("CarlErpItemIssueVendorCreditItem")]
    public class ItemIssueVendorCreditItem : AuditedEntity<Guid>, IMayHaveTenant
    {
        public Guid ItemIssueVendorCreditId { get; private set; }
        public ItemIssueVendorCredit ItemIssueVendorCredit { get; private set; }

        public Guid? VendorCreditItemId { get; private set; }
        public VendorCreditDetail VendorCreditItem { get; private set; }

        public Guid? ItemReceiptPurchaseItemId { get; private set; }
        public ItemReceiptItem ItemReceiptPurchaseItem { get; private set; }

        public Guid ItemId { get; private set; }
        public Item Item { get; private set; }

        public string Description { get; private set; }

        public decimal Qty { get; private set; }

        public decimal UnitCost { get; private set; }

        //public long TaxId { get; private set; }
        //public Tax Tax { get; private set; }

        public decimal DiscountRate { get; private set; }

        public decimal Total { get; private set; }

        public int? TenantId { get; set; }

        public long? LotId { get; private set; }
        public Lot Lot { get; private set; }

        private static ItemIssueVendorCreditItem Create(
            int? tenantId,
            long? creatorUserId,
            Guid itemId,
            string description,
            decimal qty,
            //long taxId,
            decimal unitCost,
            decimal discountRate,
            Guid? vendorCreditItemId,
            decimal total,
            Guid? itemReceiptPurchaseItemId)
        {
            return new ItemIssueVendorCreditItem
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                TenantId = tenantId,
                Description = description,
                Total = total,
                //TaxId = taxId,
                Qty = qty,
                VendorCreditItemId = vendorCreditItemId,
                DiscountRate = discountRate,
                ItemId = itemId,
                UnitCost = unitCost,
                ItemReceiptPurchaseItemId = itemReceiptPurchaseItemId,
            };
        }
        public void UpdateLot(long? lotId)
        {
            LotId = lotId;
        }
        //use for create ItemReceiptItem in Update ItemReceipt  (when we have ItemReceiptId)
        public static ItemIssueVendorCreditItem Create(
            int? tenantId,
            long? creatorUserId,
            Guid itemIssueVendorCreditId,
            Guid? vendorCreditItemId,
            Guid itemId,
            string description,
            decimal qty,
            decimal unitCost,
            //long taxId,
            decimal discountRate,
            decimal total,
            Guid? itemReceiptPurchaseItemId)
        {
            var result = Create(tenantId, creatorUserId, itemId, description, qty, unitCost, discountRate, vendorCreditItemId, total, itemReceiptPurchaseItemId);
            result.ItemIssueVendorCreditId = itemIssueVendorCreditId;
            return result;
        }

        //use for create Item Receipt Item in Update item Receipt in Create new item Receipt(when this ItemReceiptItem does not have Id yet)       
        public static ItemIssueVendorCreditItem Create(
            int? tenantId,
            long? creatorUserId,
            ItemIssueVendorCredit itemIssue,
            Guid? customerCreditItemId,
            Guid itemId,
            string description,
            //long taxId,
            decimal qty,
            decimal unitCost,
            decimal discountRate,
            decimal total,
            Guid? itemReceiptPurchaseItemId)
        {
            var result = Create(tenantId, creatorUserId, itemId, description, qty, unitCost, discountRate, customerCreditItemId, total, itemReceiptPurchaseItemId);
            result.ItemIssueVendorCredit = itemIssue;
            return result;
        }

        public void UpdateVendorCreditItemId(Guid? itemId)
        {
            this.VendorCreditItemId = itemId;
        }

        public void UpdateQty (decimal qty)
        {
            this.Qty = qty;
        }

        public void UpdateItemIssueItem(decimal unitCost, decimal discountRate, decimal total)
        {
            Total = total;
            UnitCost = unitCost;
            DiscountRate = discountRate;
        }

        // ItemReceipt cannot just change ItemReceipt
        public void Update(long lastModifiedUserId,
            Guid itemId,
            string description,
            decimal qty,
            decimal unitCost,
            decimal discountRate,
            decimal total
        //long taxId
        )
        {
            LastModifierUserId = lastModifiedUserId;
            Description = description;
            Total = total;
            Qty = qty;
            DiscountRate = discountRate;
            ItemId = itemId;
            UnitCost = unitCost;
            //TaxId = taxId;
        }
        public void UpdateUnitCost(
            decimal unitCost,
            decimal total
        )
        {
            Total = total;
            UnitCost = unitCost;
        }

    }
}
