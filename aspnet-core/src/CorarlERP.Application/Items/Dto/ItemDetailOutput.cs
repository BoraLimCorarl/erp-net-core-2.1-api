using Abp.AutoMapper;
using CorarlERP.AccountTypes.Dto;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.ItemTypes.Dto;
using CorarlERP.Lots.Dto;
using CorarlERP.SubItems.Dto;
using CorarlERP.Taxes;
using CorarlERP.Taxes.Dto;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Items.Dto
{
    [AutoMapFrom(typeof(Item))]
    public class ItemDetailOutput
    {
        public Guid Id { get; set; }

        public bool IsActive { get; set; }

        public string ItemName { get; set; }

        public string ItemCode { get; set; }

        public string Description { get; set; }

        public decimal? PurchaseCost { get; set; }

        public decimal? SalePrice { get; set; }

        public decimal? ReorderPoint { get; set; }

        public bool TrackSerial { get; set; }
        
        public long? SaleTaxId { get; set; }
        public TaxDetailOutput SaleTax { get; set; }

        public long? PurchaseTaxId { get; set; }
        public TaxDetailOutput PurchaseTax { get; set; }

        public Guid? SaleAccountId { get; set; }
        public ChartAccountDetailOutput SaleAccount { get; set; }

        public Guid? InventoryAccountId { get; set; }
        public ChartAccountDetailOutput InventoryAccount { get; set; }

        public Guid? PurchaseAccountId { get; set; }
        public ChartAccountDetailOutput PurchaseAccount { get; set; }

        public long ItemTypeId { get; set; }
        public ItemTypeDetailOutput ItemType { get; set; }

        public long? PurchaseCurrencyId { get; set; }
        public CurrencyDetailOutput PurchaseCurrency{ get; set; }

        public long? SaleCurrenyId { get; set; }
        public CurrencyDetailOutput SaleCurrency { get; set; }

        //public ItemSummaryOutput ParentItem { get; set; }
       
        public List<ItemPropertyDetailOutput> Properties { get; set; }
       // public List <SubItemDetailOuput> SubItems { get; set; }

        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public Member Member { get; set; }
        public List<GroupItems> UserGroups { get; set; }
        public bool ShowSubItems { get; set; }
        public long? DefaultLotId { get; set; }
        public List<ItemLotDto> DefaultLots { get; set; }

        public string Barcode { get; set; }
        public Guid? ImageId { get; set; }
        public bool UseBatchNo { get; set; }
        public bool AutoBatchNo { get; set; }
        public long? BatchNoFormulaId { get; set; }
        public string BatchNoFormulaName { get; set; }
        public bool TrackExpiration { get; set; }
        public bool BarcodeSameAsItemCode { get; set; }    
    }

    [AutoMapFrom(typeof(Item))]
    public class ItemSummaryOutput
    {
        public Guid Id { get; set; }

        public string ItemName { get; set; }
        public long ItemTypeId { get; set; }

        public string ItemCode { get; set; }
        public long? PurchaseTaxId { get;  set; }
        public TaxDetailOutput PurchaseTax { get;  set; }

        public Guid? InventoryAccountId { get; set; }
        public ChartAccountDetailOutput InventoryAccount { get; set; }

        public Guid? PurchaseAccountId { get; set; }
        public ChartAccountDetailOutput PurchaseAccount { get; set; }
        public decimal? SalePrice { get; set; }
        public long? SaleTaxId { get; set; }
        public TaxDetailOutput SaleTax { get; set; }
        public Guid? SaleAccountId { get; set; }
        public ChartAccountDetailOutput SaleAccount { get; set; }
        public bool ShowSubItems { get; set; }
        public string Barcode { get; set; }

    }
    [AutoMapFrom(typeof(Item))]
    public class ItemSummaryDetailOutput
    {
        public Guid? PurhchseOrderId { get; set; }
        public Guid Id { get; set; }
        public long ItemTypeId { get; set; }
        public string ItemName { get; set; }

        public string ItemCode { get; set; }

        public decimal? SalePrice { get; set; }
        public Guid? InventoryAccountId { get; set; }
        public ChartAccountDetailOutput InventoryAccount { get; set; }

        public Guid? PurchaseAccountId { get; set; }
        public ChartAccountDetailOutput PurchaseAccount { get; set; }
        public Guid? SaleAccountId { get; set; }
        public ChartAccountDetailOutput SaleAccount { get; set; }
        public long? SaleTaxId { get; set; }
        public TaxDetailOutput SaleTax { get; set; }
        public decimal AverageCost { get; set; }
        public decimal QtyOnHand { get; set; }
        public long? PurchaseTaxId { get; set; }
        public TaxDetailOutput PurchaseTax { get; set; }
        public bool ShowSubItems { get; set; }
        public bool UseBatchNo { get; set; }
        public bool AutoBatchNo { get; set; }
        public bool TrackExpiration { get; set; }
        public bool TrackSerial { get; set; }
        public string Barcode { get; set; }
    }

    public class ItemSummaryWithAccount
    {
        public Guid Id { get; set; }
        public long ItemTypeId { get; set; }
        public bool ManageInventory { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public Guid? InventoryAccountId { get; set; }
        public long? PurchaseTaxId { get; set; }
        public Guid? PurchaseAccountId { get; set; }
        public long? SaleTaxId { get; set; }
        public Guid? SaleAccountId { get; set; }
        public bool ShowSubItems { get; set; }
        public bool TrackSerial { get; set; }
        public bool UseBatchNo { get; set; }
        public bool TrackExpiration { get; set; }
        public string ItemTypeName { get; set; }

    }

    public class ItemSummaryDto
    {
        public Guid Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
    }

    public class JournalRefWithPartnerDto
    {
        public string JournalNo { get; set; }
        public string Reference { get; set; }
        public Guid PartnerId { get; set; }
    }
}
