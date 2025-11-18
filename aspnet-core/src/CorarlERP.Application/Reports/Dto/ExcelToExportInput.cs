using CorarlERP.Currencies.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{
    public class ExcelHelperInput
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public ReportOutput ReportOutput { get; set; }
    }

    public class ExcelToExportInput
    {
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string VendorName { get; set; }
        public string VendorCode { get; set; }

        public string LocationName { get; set; }

        public string AccountName { get; set; }
        public string AccountCode { get; set; }

        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public decimal Qty { get; set; }

        public string TaxName { get; set; }
        public decimal TaxRate { get; set; }
        public int RoundingDigit { get; set; }

        public string JournalNo { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public string JournalMemo { get; set; }
        public JournalType? JournalType { get; set; }
        public TransactionStatus JournalStatus { get; set; }
        public DateTime Date { get; set; }


        public decimal Current { get; set; }
        public decimal Col30 { get; set; }
        public decimal Col60 { get; set; }
        public decimal Col90 { get; set; }
        public decimal Col90Up { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public decimal LastPaymentAmount { get; set; }
        public int Aging { get; set; }
        public decimal TotalPaid { get; set; }
        public string  Currency { get; set; }


        public Dictionary<string, decimal> Currents { get; set; }
        public Dictionary<string, decimal> Col30s { get; set; }
        public Dictionary<string, decimal> Col60s { get; set; }
        public Dictionary<string, decimal> Col90s { get; set; }
        public Dictionary<string, decimal> Col90Ups { get; set; }
        public Dictionary<string, decimal> TotalAmounts { get; set; }
        public Dictionary<string, decimal> LastPaymentAmounts { get; set; }
        public Dictionary<string, decimal> Totals { get; set; }
    }
}
