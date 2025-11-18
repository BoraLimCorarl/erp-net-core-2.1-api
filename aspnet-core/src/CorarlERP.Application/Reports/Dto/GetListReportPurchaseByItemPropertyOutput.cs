using System;

namespace CorarlERP.Reports.Dto
{   
    public class GetListPurchaseByItemPropertyReportOutput
    {

        public long? CreationTimeIndex { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime Date { get; set; }
        public string JournalNo { get; set; }
        public string Reference { get; set; }
        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorType { get; set; }
        public long? VendorTypeId { get; set; }

        public long LocationId { get; set; }
        public string LocationName { get; set; }

        public Guid? ItemId { get; set; }

        public decimal Total { get; set; }
        public decimal Qty { get; set; }

        public int RoundingDigit { get; set; }
        public string User { get; set; }
        public decimal NetWeight { get; set; }
        public string SummaryBy { get; set; }
        public Guid BillId { get; set; }
        public bool IsItem { get; set; }
        public string Description { get; set; }
    }

    public class PurchaseByItemPropertyColumnSummaryOutPut
    {
        public decimal TotalQty { get; set; }
        public decimal TotalNetWeight { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
