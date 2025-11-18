using Abp.AutoMapper;
using CorarlERP.Addresses;
using CorarlERP.Currencies.Dto;
using CorarlERP.Customers.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.KitchenOrders.Dto
{
  public  class CreateKitchenOrderInput
    {
        public TransactionStatus Status { get; set; }
        public Guid? CustomerId { get; set; }
        public CAddress BillingAddress { get; set; }
        public CAddress ShippingAddress { get; set; }
        public decimal MultiCurrencyTax { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string Reference { get; set; }
        public long CurrencyId { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string Memo { get; set; }
        public bool SameAsShippingAddress { get; set; }
        public bool IsActive { get; set; }        
        public long? LocationId { get; set; }
        public decimal MultiCurrencySubTotal { get; set; }
        public decimal MultiCurrencyTotal { get; set; }
        public long? MultiCurrencyId { get; set; }
        public long? ClassId { get; set; }
        public bool IsConfirm { get; set; }
        public long? TransactionTypeId { get; set; }
        public long? PermissionLockId { get; set; }
        public List<CreateOrUpdateKitchenOrderItemInput> OrderItems { get; set; }

    }

    public class UpdateKitchenOrderInput : CreateKitchenOrderInput {  
     public Guid Id { get; set; }
    
    }



    public class GetDetailKitchenOrder
    {
        [AutoMapFrom(typeof(KitchenOrder))]
        public class KitchendOrderDetailOutput
        {
            public Guid Id { get; set; }
            public CAddress BillingAddress { get; set; }
            public CAddress ShippingAddress { get; set; }
            public string OrderNumber { get; set; }
            public DateTime OrderDate { get; set; }
            public string Reference { get; set; }
            public TransactionStatus Status { get; set; }      
            public string StatusCode { get; set; }
            public Guid? CustomerId { get; set; }
            public CustomerSummaryOutput Customer { get; set; }
            public decimal MultiCurrencyTax { get; set; }
            public long CurrencyId { get; set; }
            public CurrencyDetailOutput Currency { get; set; }
            public long? MultiCurrencyId { get; set; }
            public CurrencyDetailOutput MultiCurrency { get; set; }
            public decimal MultiCurrencyTotal { get; set; }
            public decimal MultiCurrencySubTotal { get; set; }
            public bool SameAsShippingAddress { get; set; }
            public decimal SubTotal { get; set; }
            public decimal Tax { get; set; }
            public decimal Total { get; set; }
            public string Memo { get; set; }         
            public string Location { get; set; }
            public long? LocationId { get; set; }
            public string UserName { get; set; }
            public string ClassName { get; set; }
            public long? ClassId { get; set; }
            public List<CreateOrUpdateKitchenOrderItemInput> OrderItems { get; set; }
        }
      

    }
}
