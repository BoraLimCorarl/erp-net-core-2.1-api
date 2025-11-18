using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CorarlERP.BatchNos;
using CorarlERP.ItemIssues;
using CorarlERP.ItemReceipts;
using CorarlERP.Items;
using CorarlERP.Lots;
using CorarlERP.Taxes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.VendorCredit
{
    [Table("CarlErpVendorCreditDetails")]
    public class VendorCreditDetail : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public Guid VendorCreditId { get; private set; }
        public VendorCredit VendorCredit { get; private set; }
       
        public Guid? ItemId { get; private set; }
        public Item Item { get; private set; }
        
        public long TaxId { get; private set; }
        public Tax Tax { get; private set; }

        public string Description { get; private set; }

        public decimal Qty { get; private set; }

        public decimal UnitCost { get; private set; }

        public decimal DiscountRate { get; private set; }

        public decimal Total { get; private set; }
        public void SetTotal(decimal total) => Total = total;

        public decimal MultiCurrencyUnitCost { get; private set; }
        public decimal MultiCurrencyTotal { get; private set; }

        public Lot Lot { get; private set; }
        public long? LotId { get; private set ; }

        public ItemReceiptItem ItemReceiptItem { get; private set; }
        public Guid? ItemReceiptItemId { get; private set; }

        public decimal? PurchaseCost { get; private set; }

        public void UpdateLot (long? lotId)
        {
            this.LotId = lotId;
        }

        private static VendorCreditDetail Create(int? tenantId, long? creatorUserId,
            long taxId, Guid? itemId, string description, decimal qty, decimal unitCost,
            decimal discountRate, decimal total, decimal multiCurrencyUntiCost, decimal multiCurrencyTotal,Guid? itemReceiptItemId, decimal? purchaseCost)
        {
            return new VendorCreditDetail
            {
                ItemReceiptItemId = itemReceiptItemId,
                Id = Guid.NewGuid(),
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
                PurchaseCost = purchaseCost
            };
        }

     
        public static VendorCreditDetail Create(
            int? tenantId,
            long? creatorUserId,
            VendorCredit vendorCredit,
            long taxId,
            Guid? itemId,
            string description,
            decimal qty,
            decimal unitCost,
            decimal discountRate,
            decimal total,
            decimal multiCurrencyUnitCost,
            decimal multiCurrencyTotal,
            Guid? itemReceiptItemId,
            decimal? purchaseCost)
        {
            var result = Create(tenantId, creatorUserId, taxId, itemId, description, qty, unitCost, discountRate, total, multiCurrencyUnitCost, multiCurrencyTotal,itemReceiptItemId,purchaseCost);
            result.VendorCredit = vendorCredit;
            return result;

        }
        

        public static VendorCreditDetail Create(
            int? tenantId,
            long? creatorUserId,
            Guid vendorCreditId,
            long taxId,
            Guid? itemId,
            string description,
            decimal qty,
            decimal unitCost,
            decimal discountRate,
            decimal total,
            decimal multiCurrencyUnitCost,
            decimal multiCurrencyTotal,
            Guid? itemReceiptItemId,
            decimal? purchaseCost
            )
        {
            var result = Create(tenantId, creatorUserId, taxId, itemId, description, qty, unitCost, discountRate, total, multiCurrencyUnitCost, multiCurrencyTotal,itemReceiptItemId,purchaseCost);
            result.VendorCreditId = vendorCreditId;
            return result;

        }

        public void Update(
            long lastModifiedUserId,
            long taxId, 
            Guid? itemId,
            string description, 
            decimal qty, 
            decimal unitCost, 
            decimal discountRate, 
            decimal total,
            decimal multiCurrencyUnitCost,
            decimal multiCurrencyTotal,
            Guid? itemReceiptItemId,
            decimal? purchaseCost)
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
            ItemReceiptItemId = itemReceiptItemId;
            PurchaseCost = purchaseCost;
        }
    }
}
