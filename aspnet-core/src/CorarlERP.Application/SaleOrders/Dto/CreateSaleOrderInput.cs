using CorarlERP.Addresses;
using CorarlERP.DeliverySchedules.Dto;
using CorarlERP.Exchanges.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.SaleOrders.Dto
{
    public class CreateSaleOrderInput
    {
        public TransactionStatus Status { get; set; }
        public Guid CustomerId { get; set; }
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
        public DateTime ETD { get; set; }
        public long? SaleTypeId { get; set; }
        public long? LocationId { get; set; }

        public decimal MultiCurrencySubTotal { get; set; }
        public decimal MultiCurrencyTotal { get; set; }
        public long? MultiCurrencyId { get; set; }
        public List<CreateOrUpdateSaleOrderItemInput> SaleOrderItems { get; set; }
        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }
        public bool UseExchangeRate { get; set; }
        public GetExchangeRateDto ExchangeRate { get; set; }

        public List<CreateDeliveryScheduleInput> DeliverySchedules { get; set; }
    }
}
