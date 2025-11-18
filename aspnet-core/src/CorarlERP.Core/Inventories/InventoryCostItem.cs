using CorarlERP.BatchNos;
using CorarlERP.Inventories.Data;
using CorarlERP.Items;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Inventories
{
    public class InventoryCostItem
    {
        public Guid Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal SalePrice { get; set; }
        public decimal QtyOnHand { get; set; }
        public decimal TotalCost { get; set; }
        public decimal DisplayTotalCost { get => QtyOnHand == 0 ? 0 : TotalCost; }

        public Guid InventoryAccountId { get; set; }
        public string InventoryAccountName { get; set; }
        public string InventoryAccountCode { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public long LotId { get; set; }
        public string LotName { get; set; }
        public int RoundingDigit { get; set; }
        public int RoundingDigitUnitCost { get; set; }
        public decimal AverageCost { get; set; }

        public long ItemTypeId { get; set; }
        public string ItemTypeName { get; set; }

        public UnitDto Unit { get; set; }

        public decimal NetWeight { get => Unit == null ? 0 : Unit.NetWeight * QtyOnHand; }

        public decimal TotalLocationQty { get; set; }
        public decimal TotalLocationCost { get; set; }
        
    }

}

public class GetListPropertyFilter
{
    public long PropertyId { get; set; }
    public List<long?> PropertyValueIds { get; set; }
}

public class GetListPropertyFilterTest
{
    public long PropertyId { get; set; }
    public List<long> PropertyValueIds { get; set; }
}


public class ItemDto
{
    public IEnumerable<ItemPropertySummary> Properties { get; set; }
    public long? CreationTimeIndex { get; set; }
    public Guid? TransferOrderItemId { get; set; }
    public DateTime JournalDate { get; set; }
    public string JournalMemo { get; set; }
    public string JournalReference { get; set; }
    public string PartnerName { get; set; }
    public Guid? JournalId { get; set; }
    public JournalType? JournalType { get; set; }
    public decimal UnitCost { get; set; }
    public Guid? TransactionItemId { get; set; }
    public bool IsPurchase { get; set; }
    public string UserName { get; set; }
    public long UserId { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; }
    public string ItemCode { get; set; }

    public Guid? TransactionId { get; set; }
    public Guid InventoryAccountId { get; set; }
    public string InventoryAccountCode { get; set; }
    public string InventoryAccountName { get; set; }

    public long LocationId { get; set; }
    public decimal TotalCost { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal Qty { get; set; }
    public string LocationName { get; set; }

    public int RoundingDigit { get; set; }
    public int RoundingDigitUnitCost { get; set; }
    public string JournalNo { get; set; }
    public Guid? OrderId { get; set; }
    public string ProductionProcessName { get; set; }
    public string TransactionNo { get; set; }
    public UnitDto Unit { get; set; }

    public decimal PurchaseCost { get;set;}

    public long LotId { get; set; }
    public string LotName { get; set; }

    public long ItemTypeId { get; set; }
    public string ItemTypeName { get; set; }


    public decimal NetWeight { get => Unit == null ? 0 : Unit.NetWeight * Qty; }

    public decimal TotalLocationCost { get; set; }
    public decimal TotalLocationQty { get; set; }
    public DateTime CreationTime { get; set; }
    public string Description { get; set; }
    public string OrderIndex { get; set; }
}

public class ItemTransactionDto
{
    public long? CreationTimeIndex { get; set; }

    public DateTime JournalDate { get; set; }
    public string JournalMemo { get; set; }
    public string JournalReference { get; set; }
    public string PartnerName { get; set; }
    public Guid? JournalId { get; set; }
    public JournalType? JournalType { get; set; }
    public bool IsPurchase { get; set; }
    public string UserName { get; set; }
    public long UserId { get; set; }
    public Guid ItemId { get; set; }
    //public string ItemName { get; set; }
    //public string ItemCode { get; set; }

    public Guid? TransactionId { get; set; }
    public Guid? TransactionItemId { get; set; }
    //public Guid InventoryAccountId { get; set; }
    //public string InventoryAccountCode { get; set; }
    //public string InventoryAccountName { get; set; }

    public long LocationId { get; set; }
    public decimal Qty { get; set; }
    public string LocationName { get; set; }
    
    public string JournalNo { get; set; }
   
    public long LotId { get; set; }
    public string LotName { get; set; }

    public string ProductionProcessName { get; set; }
    public DateTime CreationTime { get; set; }
    public string Description { get; set; }
    public List<BatchNoItemOutput> ItemBatchNos { get; set; }
    public string JournalTransactionTypeName { get; set; }
    public Guid? JournalTransactionTypeId { get; set; }
    public bool? Issue { get; set; }
}

public class ItemBalanceDto
{
    public bool IsPurchase { get; set; }
    public decimal Qty { get; set; }
    public long LotId { get; set; }
    public string LotName { get; set; }
    public long LocationId { get; set; }
    public string LocationName { get; set; }
    public Guid ItemId { get; set; }
    public decimal BeginningQty { get; set; }
    public decimal InQty { get; set; }
    public decimal OutQty { get; set; }
    public DateTime Date { get; set; }
    public Guid? BatchNoId { get; set; }
    public string BatchNumber { get; set; }
}

public class ItemCostSummaryDto
{
    public DateTime? Date { get; set; }
    public bool IsPurchase { get; set; }
    public decimal BeginningQty { get; set; }
    public decimal InQty { get; set; }
    public decimal OutQty { get; set; }
    public decimal Qty { get; set; }
    public decimal Cost { get; set; }
    public decimal TotalCost { get; set; }
    public long LocationId { get; set; }
    public string LocationName { get; set; }
    public Guid ItemId { get; set; }
    public int RoundingDigit { get; set; }
    public int RoundingDigitUnitCost { get; set; }
    public string Description { get; set; }
}


public class ItemCostAndQtySummaryDto
{
    public bool IsPurchase { get; set; }
    public decimal Qty { get; set; }
    public decimal Cost { get; set; }
    public decimal TotalCost { get; set; }
    public long LocationId { get; set; }
    public string LocationName { get; set; }
    public Guid ItemId { get; set; }
    public int RoundingDigit { get; set; }
    public int RoundingDigitUnitCost { get; set; }
    public decimal InQty { get; set; }
    public decimal OutQty { get; set; }
}

public class GetListStockBalanceReportOutput
{

    public Guid Id { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public decimal SalePrice { get; set; }
    public decimal TotalLotQty { get; set; } //Total Qty For Lot
    public decimal TotalQtyOnHand { get; set; } //Total Qty For Location
    public Guid InventoryAccountId { get; set; }
    public string InventoryAccountName { get; set; }
    public string InventoryAccountCode { get; set; }
    public long LocationId { get; set; }
    public string LocationName { get; set; }
    public string LotName { get; set; }
    public long LotId { get; set; }
    
    public decimal NetWeight { get; set; }
    public UnitDto Unit { get; set; }

    public List<ItemPropertySummary> Properties { get; set; }
}

public class UnitDto
{
    public long Id { get; set; }
    public decimal NetWeight { get; set; }
    public string Value { get; set; }


}

