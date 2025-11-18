using CorarlERP.Dto;
using CorarlERP.MultiTenancy;
using CorarlERP.Subscriptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CorarlERP.SubscriptionPayments.Dto
{
    public class SubscriptionPaymentInput 
    {
        public string ReceiptFileToken { get; set; }
        public EditionPriceOutput Edition { get; set; }
    }

    public class ServiceSubscriptionPaymentInput
    {
        public string ReceiptFileToken { get; set; }
        public long ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }
}
