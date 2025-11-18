using Abp.AutoMapper;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Customers;
using CorarlERP.Customers.Dto;
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
    public class POSDetailOutput
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public decimal MultiCurrencyTotal { get; set; }
        public decimal MulticurrencySubTotal { get; set; }
        public decimal MulticurrencyTax { get; set; }
        public string InvoiceNo { get; set; }
        public LocationSummaryOutput Location { get; set; }
        public string CurrencyCode { get; set; }

        public List<CreateOrUpdateInvoiceItemInput> InvoiceItems { get; set; }
        public decimal Charge { get; set; }
        public PaidStatuse PaidStatus { get; set; }
        public string PaidStatuseName { get; set; }
        public DeliveryStatus ReceivedStatus { get; set; }
        public string CreatorUser { get; set; }
        public string StatusName { get; set; }
        public TransactionStatus StatusCode { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public List<POSPaymentSummaryByPaymentMethodOutPut> PaymentSummaries { get; set; }
    }

    public class POSPaymentSummaryByPaymentMethodOutPut
    {
        public DateTime Date { get; set; }
        public string Icon { get; set; }
        public string PaymentMethod { get; set; }  
        public decimal Total { get; set; }
        public decimal MultiCurrencyTotal { get; set; }
    }
    
}
