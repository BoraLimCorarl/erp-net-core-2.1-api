using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Addresses;
using CorarlERP.Customers;
using CorarlERP.ItemIssues;
using CorarlERP.Locations;
using CorarlERP.TransactionTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Invoices
{
    [Table("CarlErpInvoices")]
    public class Invoice : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public ReceiveFrom ReceiveFrom { get; private set; }
        
        public Guid? ItemIssueId { get; private set; }
        public ItemIssue ItemIssue { get; private set; }
        public void SetItemIssue(Guid? itemIssueId) => ItemIssueId = itemIssueId;

        public Guid CustomerId { get; private set; }
        public Customer Customer { get; private set; }
        
        public bool SameAsShippingAddress { get; private set; }
        public CAddress BillingAddress { get; private set; }
        public CAddress ShippingAddress { get; private set; }

        public decimal SubTotal { get; private set; }
        public decimal Tax { get; private set; }
        public decimal Total { get; private set; }
        public decimal OpenBalance { get; private set; }
        public decimal TotalPaid { get; set; }
        public void SetSubTotal(decimal subTotal) { SubTotal = subTotal; }
        public void SetTax(decimal tax) { Tax = tax; }
        public void SetTotal(decimal total) { Total = total; }
        public void SetOpenBalance(decimal openBalance) { OpenBalance = openBalance; }
        public void SetMultiCurrencyOpenBalance(decimal openBalance) { MultiCurrencyOpenBalance = openBalance; }
        public void SetTotalPaid(decimal paid) { TotalPaid = paid; }

        public decimal MultiCurrencyOpenBalance { get; private set; }
        public decimal MultiCurrencyTotalPaid { get; private set; }
        public decimal MultiCurrencySubTotal { get; private set; }
        public decimal MultiCurrencyTax { get; private set; }
        public decimal MultiCurrencyTotal { get; private set; }

        public DateTime DueDate { get; private set; }
        public DateTime? ReceiveDate { get; private set; }
        public bool ConvertToItemIssue { get; private set; }

        public void SetDueDate(DateTime dueDate) => DueDate = dueDate;
        public void SetETD(DateTime etaDate) => ETD = etaDate;
        public void SetReceiveDate(DateTime? receiveDate) => ReceiveDate = receiveDate;

        public DateTime ETD { get; private set; }

        public DeliveryStatus ReceivedStatus { get; private set; }

        public PaidStatuse PaidStatus { get; private set; }

        public TransactionType TransactionTypeSale { get; private set; }
        public long? TransactionTypeSaleId { get; private set; }

        public bool IsItem { get; private set; }

        public void SetMultiCurrencySubTotal(decimal subTotal) => MultiCurrencySubTotal = subTotal;
        public void SetMultiCurrencyTotal(decimal total) => MultiCurrencyTotal = total;
        public bool UseExchangeRate { get; private set; }
      
        public static Invoice Create(
            int? tenantId,
            long creatorUserId,
            ReceiveFrom status,
            DateTime dueDate,
            Guid customerId,          
            bool sameAsShippingAddress,
            CAddress shippingAddress,
            CAddress billingAddress,
            decimal subTotal,
            decimal tax,
            decimal total,
            Guid? itemIssueId,            
            DateTime eTD,
            DateTime? receiveDate,
            bool convertToItemIssue,
             decimal multiCurrencySubTotal,
            decimal multiCurrencyTax,
            decimal multiCurrencyTotal,
            bool isItemInvoice,
            bool useExcangeRate)
        {
            return new Invoice()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                DueDate = dueDate,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                SubTotal = subTotal,
                Tax = tax,
                Total = total,
                OpenBalance = total,
                TotalPaid = 0,
              
                CustomerId = customerId,
                ReceiveFrom = status,
                ItemIssueId = itemIssueId,
                PaidStatus = PaidStatuse.Pending,
                ReceivedStatus = DeliveryStatus.ReceivePending,
                BillingAddress = new CAddress(billingAddress.CityTown, billingAddress.Country, billingAddress.PostalCode, billingAddress.Province, billingAddress.Street),
                SameAsShippingAddress = sameAsShippingAddress,
                ShippingAddress = sameAsShippingAddress ? new CAddress(billingAddress.CityTown, billingAddress.Country, billingAddress.PostalCode, billingAddress.Province, billingAddress.Street) :
                new CAddress(shippingAddress.CityTown, shippingAddress.Country, shippingAddress.PostalCode, shippingAddress.Province, shippingAddress.Street),
                ETD = eTD,
                ReceiveDate = receiveDate,
                ConvertToItemIssue = convertToItemIssue,
                MultiCurrencySubTotal = multiCurrencySubTotal,
                MultiCurrencyTax = multiCurrencyTax,
                MultiCurrencyTotal = multiCurrencyTotal,
                MultiCurrencyOpenBalance = multiCurrencyTotal,
                MultiCurrencyTotalPaid = 0,
                IsItem = isItemInvoice,
                UseExchangeRate = useExcangeRate
            };
        }

        public void UpdateMultiCurrencyTotalPaid(decimal amount)
        {
            MultiCurrencyTotalPaid += amount;
        }

        public void UpdateMultiCurrencyOpenBalance(decimal amount)
        {
            MultiCurrencyOpenBalance += amount;
        }

        public void UpdateTotalPaid(decimal amount)
        {
            TotalPaid += amount;
        }
        public void UpdateTransactionTypeId(long? transactionTypeSaleId)
        {
            TransactionTypeSaleId = transactionTypeSaleId;
        }
        public void UpdateOpenBalance(decimal amount)
        {
            OpenBalance += amount;
        }

        public void UpdateStatus(ReceiveFrom status)
        {
            ReceiveFrom = status;
        }

        public void UpdatePaidStatus(PaidStatuse status)
        {
            PaidStatus = status;
        }

        public void UpdateReceivedStatus(DeliveryStatus status)
        {
            ReceivedStatus = status;
        }

        public void UpdateItemIssueId(Guid? itemIssueId)
        {
            ItemIssueId = itemIssueId;
        }
        
        public void Update(
            long lastModifiedUserId,
            ReceiveFrom status,
            Guid customerId,
            DateTime dueDate,      
            bool sameAsShippingAddress,
            CAddress shippingAddress,
            CAddress billingAddress,
            decimal subTotal,
            decimal tax,
            decimal total,
            DateTime eTD,
            DateTime? receiveDate,
            bool convertToItemIssue,
            decimal multiCurrencySubTotal,
            decimal multiCurrencyTax,
            decimal multiCurrencyTotal,
            bool isItemInvoice)
        {
            //ItemIssueId = itemIssueId;
            DueDate = dueDate;
            SubTotal = subTotal;
            Tax = tax;
            Total = total;          
            CustomerId = customerId;
            ReceiveFrom = status;
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            SameAsShippingAddress = sameAsShippingAddress;
            BillingAddress.Update(billingAddress);
            ShippingAddress.Update(sameAsShippingAddress ? billingAddress : shippingAddress);
            ETD = eTD;
            ConvertToItemIssue = convertToItemIssue;
            ReceiveDate = receiveDate;
            MultiCurrencyTotal = multiCurrencyTotal;
            MultiCurrencyTax = multiCurrencyTax;
            MultiCurrencySubTotal = multiCurrencySubTotal;
            OpenBalance = total;
            MultiCurrencyOpenBalance = multiCurrencyTotal;
            PaidStatus = PaidStatuse.Pending;
            ReceivedStatus = DeliveryStatus.ReceivePending;
            TotalPaid = 0;
            MultiCurrencyTotalPaid = 0;
            IsItem = isItemInvoice;
        }

    }
}
