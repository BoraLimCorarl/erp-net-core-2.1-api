using CorarlERP.Currencies.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorarlERP.Reports.Dto
{
    public class GetListCustomerAgingReportOutput
    {
        public long? CreationTimeIndex { get; set; }
        public Guid CustomerId { get; set; }

        public string CustomerName { get; set; }
        //public decimal Current { get; set; }
        //public decimal Col30 { get; set; }
        //public decimal Col60 { get; set; }
        //public decimal Col90 { get; set; }
        //public decimal Col90Up { get; set; }
        //public decimal Total { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public decimal LastPaymentAmount { get; set; }
        public int Aging { get; set; }
        //public DateTime ToDate { get; set; }
        //public CurrencyDetailOutput Currency { get; set; }

        public List<ColumnTotalByCurrency> ColumnTotalByCurrencies { get; set; }


    }

    //public class ColumnTotalByCurrency
    //{
    //    public long CurrencyId { get; set; }
    //    public string CurrencyCode { get; set; }


    //    public decimal Current { get; set; }
    //    public decimal Col30 { get; set; }
    //    public decimal Col60 { get; set; }
    //    public decimal Col90 { get; set; }
    //    public decimal Col90Up { get; set; }
    //    public decimal Total { get; set; }


    //    public decimal LastPaymentAmount { get; set; }

    //}


    public class GetListCustomerAgingQuery
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal Current { get => Items == null ? 0 : Items.Sum(t => t.Aging <= 1 ? t.InvoiceBalanceAmount - t.CreditBalanceAmount : 0); }
        public decimal Col30 { get => Items == null ? 0 : Items.Sum(t => t.Aging > 1 && t.Aging <= 30 ? t.InvoiceBalanceAmount - t.CreditBalanceAmount : 0); }
        public decimal Col60 { get => Items == null ? 0 : Items.Sum(t => t.Aging > 30 && t.Aging <= 60 ? t.InvoiceBalanceAmount - t.CreditBalanceAmount : 0); }
        public decimal Col90 { get => Items == null ? 0 : Items.Sum(t => t.Aging > 60 && t.Aging <= 90 ? t.InvoiceBalanceAmount - t.CreditBalanceAmount : 0); }
        public decimal Col90Up { get => Items == null ? 0 : Items.Sum(t => t.Aging > 90 ? t.InvoiceBalanceAmount - t.CreditBalanceAmount : 0); }
        public decimal Total { get => Items == null ? 0 : Items.Sum(t => t.InvoiceBalanceAmount - t.CreditBalanceAmount); }
        public List<InvoiceDetailQuery> Items { get; set; }
        public CurrencyDetailOutput Currency { get; set; }

    }

    public class InvoiceDetailQuery
    {
        public int Aging {
            get
            {
                return Convert.ToInt16(Math.Ceiling((ToDate - InvoiceDate).TotalDays));
            }
        }

        public DateTime InvoiceDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal InvoiceBalanceAmount { get; set; }
        public decimal CreditBalanceAmount { get; set; }
    }

    public class CustomerAgingGroup
    {
        public Guid JournalId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime Date { get; set; }
        public decimal CreditBalanceAmount { get; set; }
        public decimal InvoiceBalanceAmount { get; set; }
        public DateTime ToDate { get; set; }
        
    }
}
