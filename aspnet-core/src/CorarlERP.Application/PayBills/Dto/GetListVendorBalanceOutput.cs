using Abp.Application.Services.Dto;
using Abp.Timing;
using AutoMapper.Mappers;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;

namespace CorarlERP.PayBills.Dto
{

    public class GetListVendorBalanceOutput
    {
        public Guid VendorId { get; set; }

        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string Vendor => $"{VendorCode} - {VendorName}";

        public string VendorTypeName { get; set; }
               
        public decimal Balance { get; set; }

        public DateTime? LastPaymentDate { get; set; }
        public DateTime? ToDate { get; set; }
        //Compare LastPayamentDate to the end of ToDate 
        public int Aging => LastPaymentDate.HasValue && ToDate.HasValue ? (int)(ToDate.Value.Date.AddDays(1).AddTicks(-1) - LastPaymentDate.Value).TotalDays : 0;
        public decimal? LastPayment { get; set; }

        public List<MultiCurrencyColumn> MultiCurrencCols { get; set; }
    }

    public class VendorBalanceQuery
    {
        public Guid TransactionId { get; set; }
        public Guid VendorId { get; set; }      
        public decimal Total { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public decimal? LastPayment { get; set; }
        public long CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public Guid AccountId { get; set; }
    }

}
