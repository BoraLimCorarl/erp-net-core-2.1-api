using CorarlERP.Inventories.Data;
using System;
using System.Collections.Generic;

namespace CorarlERP.Reports.Dto
{
    public class GetListReportBillOutput
    {
        public long? CreationTimeIndex { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreationTime {get;set;}

        public Guid VendorId { get; set; }
        public string  VendorName { get; set; }
        public string JournalNo { get; set; }
        public string Reference { get; set; }

        public long LocationId { get; set; }
        public string LocationName {get;set;}

        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public Guid AccountId { get; set; }
        public string ItemName { get; set; }
        public Guid? ItemId { get; set; }
        public string ItemCode { get; set; }
        public  decimal UnitCost { get; set; }
        public decimal Qty { get; set; }
        public decimal Total { get; set; }

        public string TaxName { get; set; }
        public long TaxId { get; set; }
        public decimal TaxRate { get; set; }
        public int RoundingDigit { get; set; }
        public int RoundingDigitUnitCost { get; set; }

        public string User { get; set; }
        public string VendorType { get; set; }
        public decimal NetWeight { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }
        public List<CurrencyColumnTotal> CurrencyColumnTotals { get; set; }
        public string Description { get; set; }
    }

}
