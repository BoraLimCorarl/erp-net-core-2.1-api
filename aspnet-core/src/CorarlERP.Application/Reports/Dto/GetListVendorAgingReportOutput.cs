using CorarlERP.Currencies.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorarlERP.Reports.Dto
{
    public class GetListVendorAgingReportOutput
    {

        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
        public long? CreationTimeIndex { get; set; }
        //public CurrencyDetailOutput Currency { get; set; }
        
        public DateTime? LastPaymentDate { get; set; }        
        public int Aging { get; set; }

        public List<ColumnTotalByCurrency> ColumnTotalByCurrencies { get; set; }    

    }
  
    public class ColumnTotalByCurrency
    {
        public long CurrencyId { get; set; }
        public string CurrencyCode { get; set; }


        public decimal Current { get; set; }
        public decimal Col30 { get; set; }
        public decimal Col60 { get; set; }
        public decimal Col90 { get; set; }
        public decimal Col90Up { get; set; }
        public decimal Total { get; set; }


        public decimal LastPaymentAmount { get; set; }

    }


    //public class BillDetailQuery
    //{   
    //    public CurrencyDetailOutput Currency { get; set; }

    //    public DateTime BillDate { get; set; }
    //    public DateTime ToDate { get; set; }
    //    public decimal BillBalanceAmount { get; set; }
    //    public decimal CreditBalanceAmount { get; set; }
    //}

    //public class LastPaymentOutput
    //{
    //    public Guid VendorId { get; set; }
    //    public DateTime LastPaymentDate { get; set; }
    //    public List<LastPaymentItem> Items { get; set; }
    //    public List<string> CurrencyColumns { get; set; }
    //    public Dictionary<string, decimal> PaymentAmounts
    //    {
    //        get {
    //            var result = new Dictionary<string, decimal>();
    //            foreach (var col in CurrencyColumns)
    //            {
    //                var total = Items == null ? 0 : Items.Where(s => s.CurrencyCode == col).Sum(t => t.TotalPaymentAmount);
    //                result.Add(col, total);
    //            }
    //            return result;
    //        }
    //    }
    //}

    //public class LastPaymentItem
    //{
    //    public string CurrencyCode { get; set; }
    //    public DateTime PaymentDate { get; set; }        
    //    public decimal TotalPaymentAmount { get; set; }
    //}

    public class VendorAgingGroup
    {
        public Guid JournalId { get; set; }
        public Guid VendorId { get; set; }
        public string VendorName { get; set; }
        public DateTime Date { get; set; }
        public decimal CreditBalanceAmount { get; set; }
        public decimal BillBalanceAmount { get; set; }
        public DateTime ToDate { get; set; }

    }
}
