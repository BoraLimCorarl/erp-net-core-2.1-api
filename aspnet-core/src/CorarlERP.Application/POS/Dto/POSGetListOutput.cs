using Abp.AutoMapper;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Customers.Dto;
using CorarlERP.Dto;
using CorarlERP.Invoices;
using CorarlERP.Invoices.Dto;
using CorarlERP.Items.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.Lots.Dto;
using CorarlERP.PaymentMethods.Dto;
using CorarlERP.Taxes.Dto;
using CorarlERP.TransactionTypes.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.POS.Dto
{
    [AutoMapFrom(typeof(Invoice))]
    public class POSGetListOutput
    {
      
        public string StatusName { get; set; }
        public TransactionStatus StatusCode { get; set; }
        public Guid Id { get; set; }      
     
        public long LocationId { get; set; }
        public LocationSummaryOutput Location { get; set; }
      
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public string InvoiceNo { get; set; }
        public PaidStatuse PaidStatus { get; set; }
        public string PaidStatusName { get; set; }
        public UserDto User { get; set; }
        public List<string> PaymentMethods { get; set; }
        public string CustomerName { get; set; }
    }

}
