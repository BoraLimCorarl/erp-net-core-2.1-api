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

namespace CorarlERP.CustomerCredits
{
    [Table("CarlErpCustomerCreditDetails")]
    public class CustomerCreditDetail : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public Guid CustomerCreditId { get; private set; }
        public CustomerCredit CustomerCredit { get; private set; }

        public Guid? ItemId { get; private set; }
        public Item Item { get; private set; }

        public long TaxId { get; private set; }
        public Tax Tax { get; private set; }

        public string Description { get; private set; }

        public decimal Qty { get; private set; }

        public decimal UnitCost { get; private set; }

        public decimal DiscountRate { get; private set; }

        public decimal Total { get; private set; }

        public long? LotId { get; private set; }
        public Lot Lot { get; private set; }

        public decimal MultiCurrencyUnitCost { get; private set; }
        public decimal MultiCurrencyTotal { get; private set; }

        public Guid? ItemIssueSaleItemId { get; private set; }
        public ItemIssueItem ItemIssueSaleItem { get; private set; }

        public decimal? SalePrice { get; private  set; }

        public void UpdateLot (long? lotId)
        {
            this.LotId = lotId;
        }

        public void SetUnitCost(decimal unitCost) { UnitCost = unitCost; }
        public void SetTotal(decimal total) { Total = total; }

    
        private static CustomerCreditDetail Create(int? tenantId, long? creatorUserId, long taxId, Guid?
                itemId, string description, decimal qty, decimal unitCost, decimal discountRate, decimal total,
                decimal multiCurrencyUntiCost, decimal multiCurrencyTotal,
                Guid? itemIssueSaleItemId, decimal? salePrice)
        {
            return new CustomerCreditDetail
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
                TaxId = taxId,
                MultiCurrencyTotal = multiCurrencyTotal,
                MultiCurrencyUnitCost = multiCurrencyUntiCost,
                ItemIssueSaleItemId = itemIssueSaleItemId,
                SalePrice = salePrice
            };
        }
        public static CustomerCreditDetail Create( 
            int? tenantId,
            long? creatorUserId, 
            CustomerCredit customerCredit,
            long taxId, 
            Guid? itemId, 
            string description,
            decimal qty,
            decimal unitCost,
            decimal discountRate,
            decimal total,
            decimal multiCurrencyUnitCost,
            decimal multiCurrencyTotal,
            Guid? itemIssueSaleItemId,
            decimal? salePrice
        )
        {
            var result = Create(tenantId, creatorUserId, taxId, itemId, description, qty, unitCost, discountRate, total, multiCurrencyUnitCost, multiCurrencyTotal, itemIssueSaleItemId,salePrice);
            result.CustomerCredit = customerCredit;
            return result;

        }

        public static CustomerCreditDetail Create(
            int? tenantId,
            long? creatorUserId,
            Guid customerCreditId,
            long taxId,
            Guid? itemId,
            string description,
            decimal qty,
            decimal unitCost,
            decimal discountRate,
            decimal total,
            decimal multiCurrencyUnitCost,
            decimal multiCurrencyTotal,
             Guid? itemIssueSaleItemId,
             decimal? salePrice
        )
        {
            var result = Create(tenantId, creatorUserId, taxId, itemId, description, qty, unitCost, discountRate, total, multiCurrencyUnitCost, multiCurrencyTotal,itemIssueSaleItemId,salePrice);
            result.CustomerCreditId = customerCreditId;
            return result;

        }

        public void Update(long lastModifiedUserId,long taxId,Guid? itemId,string description,decimal qty,decimal unitCost,
                    decimal discountRate,decimal total, decimal multiCurrencyUnitCost,
                    decimal multiCurrencyTotal, Guid? itemIssueSaleItemId, decimal? salePrice)
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
            ItemIssueSaleItemId = itemIssueSaleItemId;
            SalePrice = salePrice;
        }
    }
}
