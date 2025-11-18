using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Addresses;
using CorarlERP.Currencies;
using CorarlERP.Locations;
using CorarlERP.Vendors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.PurchaseOrders
{
    [Table("CarlErpPurchaseOrders")]
    public class PurchaseOrder : BaseAuditedEntity<Guid>
    {
        public const int MaxOrderNumberLength = 128;

        public int? TenantId { get; set; }

        
        public Guid VendorId { get; private set; }
        public Vendor Vendor { get; private set; }

        public CAddress BillingAddress { get; private set; }
        public CAddress ShippingAddress { get; private set; }

        [Required]
        [MaxLength(MaxOrderNumberLength)]
        public string OrderNumber { get; private set; }
        public DateTime OrderDate { get; private set; }
        public DateTime ETA { get; private set; }

        [MaxLength(MaxOrderNumberLength)]
        public string Reference { get; private set; }

        [Required]
        public long CurrencyId { get; private set; }
        public Currency Currency { get; private set; }

        public long? MultiCurrencyId { get; private  set; }
        public Currency MultiCurrency { get; private  set; }

        public decimal SubTotal { get; private set; }
        public decimal Tax { get; private set; }
        public decimal Total { get; private set; }

        public decimal MultiCurrencySubTotal { get;private  set; }
        public decimal MultiCurrencyTotal { get; private set; }
        public decimal MultiCurrencyTax { get; private set; }

        public bool SameAsShippingAddress { get; private set; }
        public string Memo { get; private set; }

        public bool IsActive { get; private set; }

        public TransactionStatus Status { get; private set; }
        public DeliveryStatus ReceiveStatus { get; private set; }
        public ApprovalStatus ApprovalStatus { get; private set; }
        public void SetApprovalStatus(ApprovalStatus status) => ApprovalStatus = status;

        public long? LocationId { get; private set; }
        public Location Location { get; private set; }

        public void UpdateReceiveStatusToPending()
        {
            ReceiveStatus = DeliveryStatus.ReceivePending;
        }
        public void UpdateReceiveStatusToPartial()
        {
            ReceiveStatus = DeliveryStatus.ReceivePartial;
        }
        public void UpdateReceiveStatusToReceiveAll()
        {
            ReceiveStatus = DeliveryStatus.ReceiveAll;
        }

        public void UpdateStatusToDraft()
        {
           Status = TransactionStatus.Draft;
        }
        public void UpdateStatusToClose()
        {
            Status = TransactionStatus.Close;
        }
        public void UpdateStatusToVoid()
        {
            Status = TransactionStatus.Void;
        }
        public void UpdateStatusToPublish()
        {
            Status = TransactionStatus.Publish;
        }

        public int ReceiveCount { get; private set; }
        public void SetReceiveCount(int count) => ReceiveCount = count;
        public bool UseExchangeRate { get; private set; }

        public static PurchaseOrder Create(int? tenantId, long creatorUserId, Guid vendorId,
            CAddress shippingAddress, CAddress billingAddress, bool sameAsShippingAddress,
            string reference, long currencyId, string orderNumber, DateTime orderDate,
            string memo, decimal tax, decimal total, decimal subTotal, TransactionStatus status, DateTime dTA, long? locationId,
            decimal multiCurrencySubTotal,decimal multiCurrencyTotal,long? multiCurrencyId,decimal multiCurrencyTax, bool useExchangeRate)
        {
            return new PurchaseOrder()
            {
                MultiCurrencyId = multiCurrencyId,
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                VendorId = vendorId,
                BillingAddress = new CAddress(billingAddress.CityTown, 
                billingAddress.Country, billingAddress.PostalCode,
                billingAddress.Province, billingAddress.Street),
                SameAsShippingAddress = sameAsShippingAddress,
                ShippingAddress = sameAsShippingAddress ? new CAddress(billingAddress.CityTown, 
                billingAddress.Country, billingAddress.PostalCode,
                billingAddress.Province, billingAddress.Street) :
                new CAddress(shippingAddress.CityTown,
                shippingAddress.Country, shippingAddress.PostalCode, 
                shippingAddress.Province, shippingAddress.Street),
                Reference = reference,
                CurrencyId = currencyId,
                OrderNumber = orderNumber,
                OrderDate = orderDate,
                Tax = tax,
                SubTotal = subTotal,
                Memo = memo,
                Total = total,
                IsActive = true,
                Status = status,
                ETA = dTA,
                LocationId = locationId,
                MultiCurrencySubTotal = multiCurrencySubTotal,
                MultiCurrencyTotal = multiCurrencyTotal,
                MultiCurrencyTax = multiCurrencyTax,
                UseExchangeRate = useExchangeRate,

            };
        }
        public void Update(long lastModifiedUserId, Guid vendorId,
            string reference, long currencyId, string orderNumber,
            DateTime orderDate,string memo, CAddress shippingAddress, 
            CAddress billingAddress, bool sameAsShippingAddress, 
            decimal subTotal ,decimal tax,decimal total, TransactionStatus status, DateTime dTA,long? locationId, 
            decimal multiCurrencySubTotal,
            decimal multiCurrencyTotal,long? multiCurrencyId, decimal multiCurrencyTax, bool useExchangeRate)
        {
            MultiCurrencyTax = multiCurrencyTax;
            MultiCurrencyId = multiCurrencyId;
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            VendorId = vendorId;
            BillingAddress.Update(billingAddress);
            ShippingAddress.Update(sameAsShippingAddress ? billingAddress : shippingAddress);
            Reference = reference;
            CurrencyId = currencyId;
            OrderNumber = orderNumber;
            OrderDate = orderDate;
            Tax = tax;
            Total = total;
            SubTotal = subTotal;
            Memo = memo;
            SameAsShippingAddress = sameAsShippingAddress;
            Status = status;
            ETA = dTA;
            LocationId = locationId;
            MultiCurrencyTotal = multiCurrencyTotal;
            MultiCurrencySubTotal = multiCurrencySubTotal;
            UseExchangeRate = useExchangeRate;

        }
        public void Enable(bool isEnable)
        {
            IsActive = isEnable;
        }
    }
}
