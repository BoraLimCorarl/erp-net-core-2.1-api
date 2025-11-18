using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CorarlERP.BatchNos;
using CorarlERP.CustomerCredits;
using CorarlERP.ItemIssues;
using CorarlERP.Items;
using CorarlERP.Lots;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.ItemReceiptCustomerCredits
{
    [Table("CarlErpItemReceiptCustomerCreditItem")]
    public class ItemReceiptItemCustomerCredit : AuditedEntity<Guid>, IMayHaveTenant
    {
        public Guid ItemReceiptCustomerCreditId { get; private set; }
        public virtual ItemReceiptCustomerCredit ItemReceiptCustomerCredit { get; private set; }

        public Guid? CustomerCreditItemId { get; private set; }
        public virtual CustomerCreditDetail CustomerCreditItem { get; private set; }

        public Guid ItemId { get; private set; }
        public virtual Item Item { get; private set; }

        public string Description { get; private set; }

        public decimal Qty { get; private set; }

        public decimal UnitCost { get; private set; }

        public decimal DiscountRate { get; private set; }

        public decimal Total { get; private set; }

        public int? TenantId { get; set; }

        public long? LotId { get; private set; }
        public Lot Lot { get; private set; }

        public Guid? ItemIssueSaleItemId { get; private set; }
        public ItemIssueItem ItemIssueSaleItem { get; private set; }

        private static ItemReceiptItemCustomerCredit Create(
            int? tenantId,
            long? creatorUserId, 
            Guid itemId, 
            string description, 
            decimal qty, 
            decimal unitCost, 
            decimal discountRate,
            Guid? customerCreditItemId,
            decimal total,
            Guid? itemIssueSaleItemId)
        {
            return new ItemReceiptItemCustomerCredit
            {
                ItemIssueSaleItemId = itemIssueSaleItemId,
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                TenantId = tenantId,
                Description = description,
                Total = total,
                Qty = qty,
                CustomerCreditItemId = customerCreditItemId,
                DiscountRate = discountRate,
                ItemId = itemId,
                UnitCost = unitCost
            };
        }

        public void UpdateLot(long? lot)
        {
            LotId = lot;
        }
        //use for create ItemReceiptItem in Update ItemReceipt  (when we have ItemReceiptId)
        public static ItemReceiptItemCustomerCredit Create(
            int? tenantId,
            long? creatorUserId,
            Guid itemReceiptCustomerCreditId,
            Guid? customerCreditItemId,
            Guid itemId,
            string description,
            decimal qty,
            decimal unitCost,
            decimal discountRate,
            decimal total,
            Guid? itemIssueSaleItemId)
        {
            var result = Create(tenantId, creatorUserId, itemId, description, qty, unitCost, discountRate, customerCreditItemId, total, itemIssueSaleItemId);
            result.ItemReceiptCustomerCreditId = itemReceiptCustomerCreditId;
            return result;
        }

        //use for create Item Receipt Item in Update item Receipt in Create new item Receipt(when this ItemReceiptItem does not have Id yet)       
        public static ItemReceiptItemCustomerCredit Create(
            int? tenantId,
            long? creatorUserId,
            ItemReceiptCustomerCredit itemReceipt,
            Guid? customerCreditItemId,
            Guid itemId,
            string description,
            decimal qty,
            decimal unitCost,
            decimal discountRate,
            decimal total,
            Guid? itemIssueSaleItemId)
        {
            var result = Create(tenantId, creatorUserId, itemId, description, qty, unitCost, discountRate, customerCreditItemId, total, itemIssueSaleItemId);
            result.ItemReceiptCustomerCredit = itemReceipt;
            return result;
        }

        public void UpdateCustomerCreditItemId(Guid? itemId)
        {
            this.CustomerCreditItemId = itemId;
        }
        public void UpdateQty (decimal qty)
        {
            this.Qty = qty;
        }

        // ItemReceipt cannot just change ItemReceipt
        public void Update(long lastModifiedUserId,
            Guid itemId,
            string description, decimal qty, decimal unitCost, decimal discountRate, decimal total,
             Guid? itemIssueSaleItemId)
        {
            ItemIssueSaleItemId = itemIssueSaleItemId;
            LastModifierUserId = lastModifiedUserId;
            Description = description;
            Total = total;
            Qty = qty;
            DiscountRate = discountRate;
            ItemId = itemId;
            UnitCost = unitCost;
        }

       

        public void UpdateItemReceiptItem(decimal unitCost, decimal discountRate, decimal total)
        {
            Total = total;
            UnitCost = unitCost;
            DiscountRate = discountRate;
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
