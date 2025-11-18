using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.Inventories.Data;
using CorarlERP.ItemIssues;
using CorarlERP.ItemIssueVendorCredits;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.ItemReceipts;
using CorarlERP.Items;
using CorarlERP.Items.Dto;
using CorarlERP.Journals;
using CorarlERP.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{
    public class GetInventoryInput : PagedSortedAndFilteredInputDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid?> InventoryAccount { get; set; }
        public List<long> AccountTypes { get; set; }
        public List<long?> Location { get; set; }
        public List<long?> Lots { get; set; }
        public List<Guid> Items { get; set; }
        public List<InventoryStockBalanceStatus> StockBalance { get; set; }
        public List<InventoryMovementStatus?> StockMovement { get; set; }
        public bool UsePagination { get; set; }
        public bool IsLoadMore { get; set; }
        //public List<long?> ItemProperties { get; set; }
        public List<GetListPropertyFilter> PropertyDics { get; set; }
        public List<string> ColumnNamesToSum { get; set; }
        public string GroupBy { get; set; }
    }
    public class GetInventoryReportInput: GetInventoryInput
    {
        public ReportOutput ReportOutput { get; set; }
    }

    public class GetListInventoryReportOutput
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
    
        public List<InventoryReportItemOutput> Items { get; set; }
    }


  


    public class GetListInventoryReportOutputNew
    {
        public InventoryStockBalanceStatus StockBalanceStatus { get; set; }
        public Guid Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal SalePrice { get; set; }
        public decimal TotalLotQty { get; set; } //Total Qty For Lot
        public decimal TotalQtyOnHand { get; set; } //Total Qty For Location
        public decimal TotalInQty { get; set; }
        public decimal TotalOutQty { get; set; }
        public decimal Beginning { get; set; }
        public decimal TotalCost { get; set; }
        public decimal DisplayTotalCost { get => TotalQtyOnHand == 0 ? 0 : TotalCost; }
        public Guid InventoryAccountId { get; set; }
        public string InventoryAccountName { get; set; }
        public string InventoryAccountCode { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public string LotName { get; set; }
        public long LotId { get; set; }
        public int RoundingDigit { get; set; }

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

        //public List<List<ItemPropertySummary>> Properties { get; set; }
        public List<ItemPropertySummary> PropertySummary { get; set; }
    }

    public class GetListInventoryReportGroupByOutput
    {
        public string KeyName { get; set; }
        public List<GetListInventoryReportOutputNew> Items { get; set; }
    }

    public class InventoryReportItemOutput
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


    public class InventoryItemDto
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

    public class ItemReceiptQueryOutput
    {
        public ItemReceipt ItemReceipt { get; set; }
        public ItemReceiptItem ItemReceiptItem { get; set; }
        public Journal Journal { get; set; }
        public JournalItem JournalItem { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
    }

    public class ItemReceiptCustomerCreditQueryOutput
    {
        public ItemReceiptCustomerCredit ItemReceipt { get; set; }
        public ItemReceiptItemCustomerCredit ItemReceiptItem { get; set; }
        public Journal Journal { get; set; }
        public JournalItem JournalItem { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public decimal PurchaseCost { get; set; }
    }

    public class ItemIssueQueryOutput
    {
        public ItemIssue ItemIssue { get; set; }
        public ItemIssueItem ItemIssueItem { get; set; }
        public Journal Journal { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
    }

    public class ItemIssueVendorCreditQueryOutput
    {
        public ItemIssueVendorCredit ItemIssue { get; set; }
        public ItemIssueVendorCreditItem ItemIssueItem { get; set; }
        public Journal Journal { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
    }

}
