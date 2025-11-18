using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Inventories.Data
{
    public class InventoryValuationDetail
    {
        public long? CreationTimeIndex { get; set; }
        public long LotId { get; set; }
        public string LotName { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public Guid InventoryAccountId { get; set; }
        public string InventoryAccountName { get; set; }
        public string InventoryAccountCode { get; set; }
        public bool IsPurchase { get; set; }

        public long LocationId { get; set; }
        public string LocationName { get; set; }

        public decimal TotalQty { get; set; }
        public decimal TotalCost { get; set; }
        public decimal CurrentAvgCost { get; set; }

        public decimal SalePrice { get; set; }

        public UnitDto Unit { get; set; }

        public long ItemTypeId { get; set; }
        public string ItemTypeName { get; set; }

        public decimal NetWeight { get => Unit == null ? 0 : Unit.NetWeight * TotalQty; }
        public List<InventoryValuationDetailItem> Items { get; set; }

        public List<List<ItemPropertySummary>> Properties { get; set; }

        public decimal TotalLocationQty {get;set;}
        public decimal TotalLocationCost {get;set; }

    }

    public class PropertySummaryOutput
    {
        public Guid ItemId { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }

    }


    public class InventoryPropertySummaryOutput
    {
        public Guid ItemId { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public Guid? InventoryAccountId { get; set; }
        public string InventoryAccountName { get; set; }
        public string InventoryAccountCode { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal? SalePrice { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }

    }

    public class ItemWithPropertySummaryOutput
    {
        public Guid Id { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal? SalePrice { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }

    }

    public class ItemPropertySummary
    {
        public long Id { get; set; }
        public long PropertyId { get; set; }
        public string PropertyName { get; set; }

        public bool IsUnit { get; set; }
        public bool IsItemGroup { get; set; }

        public decimal NetWeight { get; set; }


        public long? ValueId { get; set; }
        public string Value { get; set; }
        public Guid ItemId { get; set; }
    }

    public class InventoryValuationDetailForCalculation
    {
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public Guid InventoryAccountId { get; set; }
        public string InventoryAccountName { get; set; }
        public string InventoryAccountCode { get; set; }

        public long LocationId { get; set; }
        public string LocationName { get; set; }

        public decimal TotalQty { get; set; }
        public decimal TotalCost { get; set; }
        public decimal CurrentAvgCost { get; set; }

        public decimal SalePrice { get; set; }

        public UnitDto Unit { get; set; }

        public long ItemTypeId { get; set; }
        public string ItemTypeName { get; set; }
        public decimal NetWeight { get; set; }
    }

    public class InventoryValuationDetailItem
    {
        public long? CreationTimeIndex { get; set; }
        public Guid? TransferOrderItemId { get; set; }
        public Guid? JournalId { get; set; }
        public Guid? TransactionId { get; set; }
        public Guid? TransactionItemId { get; set; }
        public DateTime Date { get; set; }
        public JournalType? JournalType { get; set; }
        public string JournalNo { get; set; }
        public string TransactionNo { get; set; }
        public string Reference { get; set; }
        public Guid ItemId { get; set; }
        public Guid InventoryAccountId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string GroupKey { get; set; }
        public decimal InQty { get; set; }
        public decimal OutQty { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LineTotal { get; set; }
        public decimal TotalQty { get; set; }
        public decimal TotalCost { get; set; }
        public decimal AVGCost { get; set; }
        public Guid? OrderId { get; set; }
        public long? LocationId { get; set; }
        public string LocationName { get; set; }

        public long LotId { get; set; }
        public string LotName { get; set; }
        public int RoundingDigit { get; set; }
        public int RoundingDigitUnitCost { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }

        public AdjustmentJournalItem AdjustmentJournalItem { get; set; }
        public string Description { get; set; }
    }

    public class InventoryValuationDetailItemGroupBy {
        public string KeyName { get; set; }
        public List<InventoryValuationDetailItem> Items { get; set; }

    }
    public class AdjustmentJournalItem
    {
        public Guid JournalId { get; set; } 
        public Guid InventoryAccountId { get; set; } 
        public string Description { get; set; }
        public decimal Total { get; set; } 
        public Guid? ItemReceiptItemId { get; set; }
    }

    
}
