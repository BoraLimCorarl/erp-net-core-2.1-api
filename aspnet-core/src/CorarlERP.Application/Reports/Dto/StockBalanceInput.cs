using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.Inventories.Data;
using CorarlERP.Items;
using CorarlERP.Items.Dto;
using CorarlERP.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{
    public class GetStockBalanceInput : PagedSortedAndFilteredInputDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid?> InventoryAccount { get; set; }
        public List<long?> AccountTypes { get; set; }
        public List<long?> Location { get; set; }
        public List<long?> Lots { get; set; }
        public List<InventoryMovementStatus?> StockMovement { get; set; }
        public bool UsePagination { get; set; }
        public bool IsLoadMore { get; set; }
        public List<Guid> Items { get; set; }
        public List<InventoryStockBalanceStatus> StockBalance { get; set; }
        public List<GetListPropertyFilter> PropertyDics { get; set; }
        public List<string> ColumnNamesToSum { get; set; }
        public string GroupBy { get; set; }
        public List<Guid> JournalTransactionTypeIds { get; set; }

    }
    public class GetStockBalanceReportInput: GetStockBalanceInput
    {
        public ReportOutput ReportOutput { get; set; }
    }

    public class CheckCalculationInput
    {
        public DateTime? FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class GetListStockBalanceReportOutput
    {
        public Guid Id { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public decimal TotalQtyOnHand
        {
            get
            {
                return Items == null || Items.Count == 0 ? 0 : Items.Sum(u => u.QtyOnHand);
            }
        }

        public decimal TotalCost
        {
            get
            {
                return Items == null || Items.Count == 0 ? 0 : Items.Sum(u => u.TotalCost);
            }
        }
    
        public List<StockBalanceReportItemOutput> Items { get; set; }
    }


  


    public class GetListStockBalanceReportOutputNew
    {
      
        public Guid Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal SalePrice { get; set; }
        public decimal TotalLotQty { get; set; } //Total Qty For Lot
        public decimal TotalQtyOnHand { get; set; } //Total Qty For Location
        public decimal TotalOutQty { get; set; }
        public decimal OutNetWeight { get; set; }
        public decimal TotalInQty { get; set; }
        public InventoryStockBalanceStatus StockBalanceStatus { get; set; }
        public decimal Beginning { get; set; }
        public decimal TotalCost { get; set; }
        public decimal DisplayTotalCost { get => TotalQtyOnHand == 0 ? 0 : TotalCost; }
        public Guid? InventoryAccountId { get; set; }
        public string InventoryAccountName { get; set; }
        public string InventoryAccountCode { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public string LotName { get; set; }
        public long LotId { get; set; }
        public int RoundingDigit { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }

        decimal _averageCost = decimal.MinValue;
        public decimal AverageCost
        {
            get
            {
                return  TotalQtyOnHand == 0 ? 0 : !_averageCost.Equals(decimal.MinValue) ? _averageCost : Math.Round(TotalCost / TotalQtyOnHand, RoundingDigitUnitCost);
            }
            set { _averageCost = value; }
        }
        

        public int RoundingDigitUnitCost { get; set; }
        public decimal NetWeight { get; set; }
        public UnitDto Unit { get; set; }

        public List<ItemPropertySummary> Properties { get; set; }
    }

    public class GetListStockBalanceReportGroupByOutput
    {
        public string KeyName { get; set; }
        public List<GetListStockBalanceReportOutputNew> Items { get; set; }
    }

    public class StockBalanceReportItemOutput
    {
        public Guid Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int RoundingDigit { get; set; }
        public decimal AverageCost {
            get {
                return QtyOnHand == 0 ? 0 : TotalCost / QtyOnHand;
            }
        }
        public decimal TotalCost { get; set; }        
        public decimal SalePrice { get; set; }
        public decimal QtyOnHand { get; set; }
        public Guid InventoryAccountId { get; set; }
    }


    public class StockBalanceItemDto
    {
        public bool IsPurchase { get; set; }
        public Item Item { get; set; }
        public Guid? TransactionId { get; set; }
        public Guid InventoryAccountId { get; set; }
        public long LocationId { get; set; }
        public decimal TotalCost { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal Qty { get; set; }
        public string Location { get; set; }
    }


    public class GetListAssetBalanceReportOutput
    {

        public Guid Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal TotalQtyOnHand { get; set; } 
        public decimal TotalOutQty { get; set; }
        public decimal TotalInQty { get; set; }
        public InventoryStockBalanceStatus StockBalanceStatus { get; set; }
        public decimal Beginning { get; set; }
        public Guid? InventoryAccountId { get; set; }
        public string InventoryAccountName { get; set; }
        public string InventoryAccountCode { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public string LotName { get; set; }
        public long LotId { get; set; }
        public int RoundingDigit { get; set; }

        public string Status { get; set; }

        public long? LinkLot { get; set; }

        public List<ItemPropertySummary> Properties { get; set; }
    }

    public class GetAssetBalanceInput : PagedSortedAndFilteredInputDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<long?> Location { get; set; }
        public List<long?> Lots { get; set; }
        public List<InventoryMovementStatus?> StockMovement { get; set; }
        public bool UsePagination { get; set; }
        public bool IsLoadMore { get; set; }
        public List<Guid> Items { get; set; }
        public List<InventoryStockBalanceStatus> StockBalance { get; set; }
        public List<GetListPropertyFilter> PropertyDics { get; set; }
        public List<string> ColumnNamesToSum { get; set; }
        public string GroupBy { get; set; }
        public List<Guid> JournalTransactionTypeIds { get; set; }
    }

    public class GetAssetBalanceReportInput : GetAssetBalanceInput
    {
        public ReportOutput ReportOutput { get; set; }
    }

    public class GetListAssetBalanceReportGroupByOutput
    {
        public string KeyName { get; set; }
        public List<GetListAssetBalanceReportOutput> Items { get; set; }
    }
}
