using Abp.AutoMapper;
using CorarlERP.AccountTypes.Dto;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.ItemTypes.Dto;
using CorarlERP.Lots.Dto;
using CorarlERP.PropertyValues.Dto;
using CorarlERP.SubItems.Dto;
using CorarlERP.Taxes;
using CorarlERP.Taxes.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Items.Dto
{
    [AutoMapFrom(typeof(Item))]
    public class ItemGetListOutput
    {
        public Guid Id { get; set; }

        public long? LotId { get; set; }
        public string LotName { get; set; }
        public bool IsDefaultLot { get; set; }
        public bool IsActive { get; set; }

        public string ItemName { get; set; }

        public string ItemCode { get; set; }

        public decimal Min { get;  set; }
        public decimal Max { get; set; }
        public ItemTypeDetailOutput ItemType { get; set; }
       
        public long? PurchaseTaxId { get; set; }
        public TaxDetailOutput PurchaseTax { get; set; }

        public Guid? InventoryAccountId { get; set; }
        public ChartAccountSummaryOutput InventoryAccount { get; set; }

        public Guid? PurchaseAccountId { get; set; }
        public ChartAccountSummaryOutput PurchaseAccount { get; set; }

        public decimal? SalePrice { get; set; }
        public long? SaleTaxId { get; set; }
        public TaxDetailOutput SaleTax { get; set; }
        public Guid? SaleAccountId { get; set; }
        public ChartAccountSummaryOutput SaleAccount { get; set; }

        public decimal AverageCost { get; set; }
        public decimal QtyOnHand { get; set; }
        public decimal? PurchaseCost { get; set; }
        public string Description { get; set; }
        public decimal? ReorderPoint { get; set; }
        public bool TrackSerial { get; set; }

        public List<PropertyOutput> Property { get; set; }
        public bool ShowSubItems { get; set; }

        public string Barcode { get; set; }
        public Guid? ImageId { get; set; }
        public bool UseBatchNo { get; set; }
        public bool AutoBatchNo { get; set; }
        public Guid? BatchNoId { get; set; }
        public string BatchNumber { get; set; }
        public bool TrackExpiration { get; set; }

#if closecode
         
          public decimal? ReorderPoint { get; set; }
          public bool TrackSerial { get; set; }
          public TaxDetailOutput SaleTax { get; set; }
          public TaxDetailOutput PurchaseTax { get; set; }
          public ChartAccountDetailOutput SaleAccount { get; set; }
          public ChartAccountDetailOutput InventoryAccount { get; set; }
          public ChartAccountDetailOutput PurchaseAccount { get; set; }
          public CurrencyDetailOutput PurchaseCurrency{ get; set; }
          public CurrencyDetailOutput SaleCurrency { get; set; }
          public ItemSummaryOutput ParentItem { get; set; }
          public List<ItemPropertyDetailOutput> Properties { get; set; }
#endif

    }

    public class ItemListIdOutput
    {
        public Guid Id { get; set; }
        public decimal QtyOnHand { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
    }

    public class ItemGetListBalanceOutput {
        public Guid Id { get; set; }
        public decimal QtyOnHand { get; set; }
        public decimal AverageCost { get; set; }
    }


    public class GetItemBalanceQtyInput
    {
        public DateTime Date { get; set; }
        public List<Guid> Items { get; set; }
        public List<long?> Locations { get; set; }
    }


    public class SubitemGetListOutput
    {
        public Guid Id { get; set; }

       

        public string ItemName { get; set; }

        public string ItemCode { get; set; }


        public ItemTypeDetailOutput ItemType { get; set; }

        public long? PurchaseTaxId { get; set; }
        public TaxDetailOutput PurchaseTax { get; set; }

        public Guid? InventoryAccountId { get; set; }
        public ChartAccountSummaryOutput InventoryAccount { get; set; }

        public Guid? PurchaseAccountId { get; set; }
        public ChartAccountSummaryOutput PurchaseAccount { get; set; }

        public decimal? SalePrice { get; set; }
        public long? SaleTaxId { get; set; }
        public TaxDetailOutput SaleTax { get; set; }
        public Guid? SaleAccountId { get; set; }
        public ChartAccountSummaryOutput SaleAccount { get; set; }

        public decimal Qty { get; set; }

        public long? LotId { get; set; }
        public string LotName { get; set; }
        public decimal QtyOnHand { get; set; }
    }

}
