using Abp.AutoMapper;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Customers.Dto;
using CorarlERP.Dto;
using CorarlERP.Vendors;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.ItemReceipts.Dto
{
    [AutoMapFrom(typeof(ItemReceipt))]
    public class GetListItemReceiptOut
    {

        public long? CreationTimeIndex { get; set; }
        public DateTime? CreationTime { get; set; }
        public bool IsCanVoidOrDraftOrClose { get; set; }
        public Guid Id { get; set; }

        public string JournalNo { get; set; }
        public JournalType Type { get; set; }
        public string TypeName { get; set; }
        public Guid? VendorId { get; set; }
        public VendorSummaryOutput Vendor { get; set; }

        public Guid? CustomerId { get; set; }
        public CustomerSummaryOutput Customer { get; set; }

        public decimal Total { get; set; }

        public DateTime Date { get; set; }

        public TransactionStatus Status { get; set; }

        public int CountItem { get; set; }
        public string Memo { get; set; }
        public string LocationName { get; set; }
        public string AccountName { get; set; }
        public Guid AccountId { get; set; }

        public UserDto User { get; set; }
        public string Reference { get; set; }
        public string JournalTransactionTypeName { get; set; }
    }


    public class GetListInventoryReportOutput
    {
        public Guid Id { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public decimal TotalQtyOnHand { get; set; }
        public decimal TotalAverageCost{ get; set; }
        public decimal TotalCost { get; set; }
        public decimal? TotalSalePrice { get; set; }
        public List<InventoryReportItemOutput> items { get; set; }
    }

    public class InventoryReportItemOutput
    {
        public Guid Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }

        public decimal QtyOnHand { get; set; }
        public decimal AverageCost { get
            {
                return QtyOnHand == 0 ? 0 : TotalCost / QtyOnHand;
            }
        }
        //public decimal AverageCost
        //{
        //    get
        //    {
        //        return TotalCost / QtyOnHand;
        //    }
        //}
        public decimal TotalCost { get; set; }
        //public decimal TotalCost
        //{
        //    get
        //    {
        //        return QtyOnHand * AverageCost;
        //    }
        //}
        public decimal? SalePrice { get; set; }

    }
}
