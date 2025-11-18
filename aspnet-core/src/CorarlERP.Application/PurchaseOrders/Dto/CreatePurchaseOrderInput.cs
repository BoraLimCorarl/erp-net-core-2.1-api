using CorarlERP.Addresses;
using CorarlERP.Exchanges.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.PurchaseOrders.PurchaseOrder;

namespace CorarlERP.PurchaseOrders.Dto
{
    public class CreatePurchaseOrderInput
    {

        public TransactionStatus Status {get;set;}
        public Guid VendorId { get; set; }        
        public CAddress BillingAddress { get; set; }
        public CAddress ShippingAddress { get; set; }    
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string Reference { get; set; }
        [Required]
        public long CurrencyId { get; set; }  
        public long? MulitCurrencyId { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public decimal MultiCurrencySubTotal { get; set; }
        public decimal MultiCurrencyTotal { get; set; }
        public string Memo { get; set; }
        public bool SameAsShippingAddress { get; set; }
        public decimal MultiCurrencyTax { get; set; }
        public bool IsActive { get; set; }
        public DateTime ETA { get; set; }
        public long? LocationId { get; set; }
        public List<CreateOrUpdatePurchaseOrderItemInput> PurchaseOrderItems { get; set; }
        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }
        public bool UseExchangeRate { get; set; }
        public GetExchangeRateDto ExchangeRate { get; set; }
    }
}
