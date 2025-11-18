using Abp.Timing;
using CorarlERP.Addresses;
using CorarlERP.Currencies;
using CorarlERP.Customers;
using CorarlERP.Locations;
using CorarlERP.TransactionTypes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.SaleOrders
{
    [Table("CarlErpSaleOrders")]
    public class SaleOrder : BaseAuditedEntity<Guid>
    {
        public const int MaxOrderNumberLength = 128;

        public int? TenantId { get; set; }


        public Guid CustomerId { get; private set; }
        public Customer Customer { get; private set; }

        public CAddress BillingAddress { get; private set; }
        public CAddress ShippingAddress { get; private set; }

        [Required]
        [MaxLength(MaxOrderNumberLength)]
        public string OrderNumber { get; private set; }
        public DateTime OrderDate { get; private set; }

        public DateTime ETD { get; private set; }

        [MaxLength(MaxOrderNumberLength)]
        public string Reference { get; private set; }

        [Required]
        public long CurrencyId { get; private set; }
        public Currency Currency { get; private set; }


        public long? MultiCurrencyId { get; private set; }
        public Currency MultiCurrency { get; private set; }

        public long? LocationId { get; private set; }
        public Location Location { get; private set; }

        public decimal SubTotal { get; private set; }
        public decimal Tax { get; private set; }
        public decimal Total { get; private set; }

        public decimal MultiCurrencySubTotal { get; private set; }
        public decimal MultiCurrencyTotal { get; private set; }
        public decimal MultiCurrencyTax { get; private set; }
        public bool SameAsShippingAddress { get; private set; }
        public string Memo { get; private set; }

        public bool IsActive { get; private set; }

        public TransactionStatus Status { get; private set; }
        public DeliveryStatus ReceiveStatus { get; private set; }
        public ApprovalStatus ApprovalStatus { get; private set; }
        public void SetApprovalStatus(ApprovalStatus status) => ApprovalStatus = status;

        public TransactionType SaleTransactionType { get; private set; }
        public long? SaleTransactionTypeId { get; private set; }


        public void UpdateSaleType (long? saleTypeId)
        {
            this.SaleTransactionTypeId = saleTypeId;
        }
        public void UpdateReceiveStatusToPending()
        {
            ReceiveStatus = DeliveryStatus.ShipPending;
        }
        public void UpdateReceiveStatusToPartial()
        {
            ReceiveStatus = DeliveryStatus.ShipPartial;
        }
        public void UpdateReceiveStatusToReceiveAll()
        {
            ReceiveStatus = DeliveryStatus.ShipAll;
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

        public int IssueCount { get; private set; }
        public void SetIssueCount(int count) => IssueCount = count;

        public bool UseExchangeRate {  get; private set; }

        public static SaleOrder Create(int? tenantId, long creatorUserId, Guid customerId,
            CAddress shippingAddress, CAddress billingAddress, bool sameAsShippingAddress,
            string reference, long currencyId, string orderNumber, DateTime orderDate,
            string memo, decimal tax, decimal total, decimal subTotal, TransactionStatus status,
            DateTime eTD,long? locationId,long? multiCurrencyId, decimal multiCurrencySubTotal,
            decimal multiCurrencyTotal,decimal multiCurrencyTax, bool useExchangeRate)
        {
            return new SaleOrder()
            {
                MultiCurrencyTax = multiCurrencyTax,
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                CustomerId = customerId,
                BillingAddress = new CAddress(billingAddress.CityTown,
                    billingAddress.Country, billingAddress.PostalCode,
                    billingAddress.Province, billingAddress.Street),
                SameAsShippingAddress = sameAsShippingAddress,
                ShippingAddress = sameAsShippingAddress ? new CAddress(billingAddress.CityTown,
                    billingAddress.Country, billingAddress.PostalCode,
                    billingAddress.Province, billingAddress.Street) :
                    new CAddress(
                        shippingAddress.CityTown,
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
                ETD = eTD,
                LocationId = locationId,
                MultiCurrencyId = multiCurrencyId,
                MultiCurrencySubTotal = multiCurrencySubTotal,
                MultiCurrencyTotal = multiCurrencyTotal,
                UseExchangeRate = useExchangeRate,
            };
        }
        public void Update(long lastModifiedUserId, Guid customerId,
            string reference, long currencyId, string orderNumber,
            DateTime orderDate, string memo, CAddress shippingAddress,
            CAddress billingAddress, bool sameAsShippingAddress,
            decimal subTotal, decimal tax, decimal total, 
            TransactionStatus status, DateTime eTD,long? locationId,
            long? multiCurrencyId, decimal multiCurrencySubTotal, decimal multiCurrencyTotal, decimal multiCurrencyTax, bool useExchangeRate)
        {
            MultiCurrencyTax = multiCurrencyTax;
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            CustomerId = customerId;
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
            ETD = eTD;
            LocationId = locationId;
            MultiCurrencyId = multiCurrencyId;
            MultiCurrencySubTotal = multiCurrencySubTotal;
            MultiCurrencyTotal = multiCurrencyTotal;
            UseExchangeRate = useExchangeRate;

        }
        public void Enable(bool isEnable)
        {
            IsActive = isEnable;
        }

    }
}
