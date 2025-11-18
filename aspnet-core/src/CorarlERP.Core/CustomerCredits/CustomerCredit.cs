using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Addresses;
using CorarlERP.Customers;
using CorarlERP.ItemIssues;
using CorarlERP.ItemReceipts;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.CustomerCredits
{

    [Table("CarlErpCustomerCredits")]
    public class CustomerCredit : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public Guid CustomerId { get; private set; }
        public virtual Customer Customer { get; private set; }

        //public long LocationId { get; private set; }
        //public Location Location { get; private set; }

        public bool SameAsShippingAddress { get; private set; }
        public CAddress BillingAddress { get; private set; }
        public CAddress ShippingAddress { get; private set; }

        public decimal SubTotal { get; private set; }
        public decimal Tax { get; private set; }
        public decimal Total { get; private set; }
        public DateTime DueDate { get; private set; }
        public decimal OpenBalance { get; private set; }
        public DeliveryStatus ShipedStatus { get; private set; }

        public void SetSubTotal(decimal subTotal) { SubTotal = subTotal; }
        public void SetTax(decimal tax) { Tax = tax; }
        public void SetTotal(decimal total) { Total = total; }
        public void SetOpenBalance(decimal openBalance) { OpenBalance = openBalance; }
        public void SetTotalPaid(decimal paid) { TotalPaid = paid; }

        public PaidStatuse PaidStatus { get; private set; }
        public decimal TotalPaid { get; private set; }

        public DateTime? ReceiveDate { get; private set; }
        public void SetDueDate(DateTime dueDate) => DueDate = dueDate;
        public void SetReceiveDate(DateTime? receiveDate) => ReceiveDate = receiveDate;
        public bool ConvertToItemReceipt { get; private set; }

        public decimal MultiCurrencyOpenBalance { get; private set; }
        public decimal MultiCurrencyTotalPaid { get; private set; }
        public decimal MultiCurrencySubTotal { get; private set; }
        public decimal MultiCurrencyTax { get; private set; }
        public decimal MultiCurrencyTotal { get; private set; }


        public ReceiveFrom ReceiveFrom { get; private set; }       
        public Guid? ItemIssueSaleId { get; private set; }
        public ItemIssue ItemIssueSale { get; private set; }

        public bool IsPOS { get; private set; }
        public bool IsItem { get; private set; }
        public bool UseExchangeRate { get; private set; }

        public static CustomerCredit Create(
           int? tenantId,
           long creatorUserId,
           Guid customerId,
         //  long locationId,
           bool sameAsShippingAddress,
           CAddress shippingAddress,
           CAddress billingAddress,
           decimal subTotal,
           decimal tax,
           decimal total,
           DateTime dueDate,
           bool convertToItemReceipt,
           DateTime? receiveDate,
           decimal multiCurrencySubTotal,
           decimal multiCurrencyTax,
           decimal multiCurrencyTotal,
           ReceiveFrom receiveFrom,          
           Guid? itemIssueSaleId,
           bool isPOS,
           bool isItem,
           bool useExchangeRate
           )
        {
            return new CustomerCredit()
            {
                ReceiveFrom = receiveFrom,              
                ItemIssueSaleId = itemIssueSaleId,
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                SubTotal = subTotal,
                TotalPaid = 0,
                Tax = tax,
                DueDate = dueDate,
                OpenBalance = total,
                Total = total,
               // LocationId = locationId,
                CustomerId = customerId,
                PaidStatus = PaidStatuse.Pending,
                ShipedStatus = DeliveryStatus.ShipPending,
                BillingAddress = new CAddress(billingAddress.CityTown, billingAddress.Country,
                billingAddress.PostalCode, billingAddress.Province, billingAddress.Street),
                SameAsShippingAddress = sameAsShippingAddress,
                ShippingAddress = sameAsShippingAddress ? new CAddress(billingAddress.CityTown,
                billingAddress.Country, billingAddress.PostalCode, billingAddress.Province, billingAddress.Street) :
                new CAddress(shippingAddress.CityTown, shippingAddress.Country,
                shippingAddress.PostalCode, shippingAddress.Province, shippingAddress.Street),
                ConvertToItemReceipt = convertToItemReceipt,
                ReceiveDate = receiveDate,

                MultiCurrencySubTotal = multiCurrencySubTotal,
                MultiCurrencyTax = multiCurrencyTax,
                MultiCurrencyTotal = multiCurrencyTotal,
                MultiCurrencyOpenBalance = multiCurrencyTotal,
                MultiCurrencyTotalPaid = 0,
                IsPOS = isPOS,
                IsItem = isItem,
                UseExchangeRate = useExchangeRate
            };
        }

        public void IncreaseMultiCurrencyOpenBalance(decimal amount)
        {
            MultiCurrencyOpenBalance += amount;
        }

        public void IncreaseMultiCurrencyTotalPaid(decimal openBalance)
        {
            MultiCurrencyTotalPaid += openBalance;
        }

        public void IncreaseTotalPaid(decimal totalPaid)
        {
            TotalPaid += totalPaid;
        }

        public void IncreaseOpenbalance(decimal openBalance)
        {
            OpenBalance += openBalance;
        }

        public void UpdatePaidStatus(PaidStatuse status)
        {
            PaidStatus = status;
        }

        public void UpdateToPaid()
        {
            PaidStatus = PaidStatuse.Paid;
        }

        public void UpdateToPartial()
        {
            PaidStatus = PaidStatuse.Partial;
        }
        public void UpdateToPending()
        {
            PaidStatus = PaidStatuse.Pending;
        }

        public void UpdateShipedStatus(DeliveryStatus status)
        {
            ShipedStatus = status;
        }

        public void Update(
            long lastModifiedUserId,
            Guid customerId,
          //  long locationId,
            bool sameAsShippingAddress,
            CAddress shippingAddress,
            CAddress billingAddress,
            decimal subTotal,
            decimal tax,
            decimal total,
            DateTime dueDate,
            bool convertToItemReceipt,
            DateTime? receiveDate,
            decimal multiCurrencySubTotal,
            decimal multiCurrencyTax,
            decimal multiCurrencyTotal,
            ReceiveFrom receiveFrom,          
            Guid? itemIssueSaleId,
            bool isPOS,
            bool isItem)
        {
            SubTotal = subTotal;
            Tax = tax;
            Total = total;
            TotalPaid = 0;
            MultiCurrencyTotalPaid = 0;
            DueDate = dueDate;
            CustomerId = customerId;
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            SameAsShippingAddress = sameAsShippingAddress;
            BillingAddress.Update(billingAddress);
            ShippingAddress.Update(sameAsShippingAddress ? billingAddress : shippingAddress);
            ConvertToItemReceipt = convertToItemReceipt;
            ReceiveDate = receiveDate;
            MultiCurrencySubTotal = multiCurrencySubTotal;
            MultiCurrencyTax = multiCurrencyTax;
            MultiCurrencyTotal = multiCurrencyTotal;
            MultiCurrencyOpenBalance = multiCurrencyTotal;
            ReceiveFrom = receiveFrom;          
            ItemIssueSaleId = itemIssueSaleId;
            OpenBalance = Total;
            PaidStatus = PaidStatuse.Pending;
            ShipedStatus = DeliveryStatus.ShipPending;
            IsPOS = isPOS;
            IsItem = isItem;
        }
    }
}
