using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Addresses;
using CorarlERP.ItemIssues;
using CorarlERP.ItemIssueVendorCredits;
using CorarlERP.ItemReceipts;
using CorarlERP.Locations;
using CorarlERP.Vendors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.VendorCredit
{
    [Table("CarlErpVendorCredit")]
    public class VendorCredit : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        
        public ReceiveFrom ReceiveFrom { get; private set; }
        
        public Guid VendorId { get; private set; }
        public Vendor Vendor { get; private set; }

        public bool SameAsShippingAddress { get; private set; }
        public CAddress BillingAddress { get; private set; }
        public CAddress ShippingAddress { get; private set; }

        public decimal SubTotal { get; private set; }
        public void SetSubTotal(decimal subTotal) => SubTotal = subTotal;
        public decimal Tax { get; private set; }
        public void SetTax(decimal tax) => Tax = tax;
        public decimal Total { get; private set; }
        public void SetTotal(decimal total) => Total = total;
        public DateTime DueDate { get; private set; }
        public decimal OpenBalance { get; private set; }
        public void SetOpenBalance(decimal openBalance) => OpenBalance = openBalance;
        public DeliveryStatus ShipedStatus { get; private set; }
        
        public PaidStatuse PaidStatus { get; private set; }
        public decimal TotalPaid { get; private set; }
        public void SetTotalPaid(decimal totalPaid) => TotalPaid = totalPaid;

        public DateTime? IssueDate { get; private set; }
        public void SetDueDate(DateTime dueDate) => DueDate = dueDate;
        public void SetIssueDate(DateTime issueDate) => IssueDate = issueDate;
        
        public bool ConvertToItemIssueVendor { get; private set; }
        
        public decimal MultiCurrencyOpenBalance { get; private set; }
        public decimal MultiCurrencyTotalPaid { get; private set; }
        public decimal MultiCurrencySubTotal { get; private set; }
        public decimal MultiCurrencyTax { get; private set; }
        public decimal MultiCurrencyTotal { get; private set; }

        public ItemReceipt ItemReceipt { get; private set; }
        public Guid? ItemReceiptId { get; private set; }
        public bool IsItem { get; private set; }
        public bool UseExchangeRate { get; private set; }

        public static VendorCredit Create(
            int? tenantId,
            long creatorUserId,
            Guid vendorId,          
            bool sameAsShippingAddress,
            CAddress shippingAddress,
            CAddress billingAddress,
            decimal subTotal,
            decimal tax,
            decimal total,
            DateTime dueDate,
            DateTime? issueDate,
            bool convertToItemIssueVendor,
            decimal multiCurrencySubTotal,
            decimal multiCurrencyTax,
            decimal multiCurrencyTotal,
            Guid? itemReceiptId,
            ReceiveFrom receiveFrom,
            bool isItem,
            bool useExchangeRate)
        {
            return new VendorCredit()
            {
                ReceiveFrom = receiveFrom,
                ItemReceiptId = itemReceiptId,
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
                VendorId = vendorId,
                PaidStatus = PaidStatuse.Pending,
                ShipedStatus = DeliveryStatus.ShipPending,
                BillingAddress = new CAddress(billingAddress.CityTown, billingAddress.Country, billingAddress.PostalCode, billingAddress.Province, billingAddress.Street),
                SameAsShippingAddress = sameAsShippingAddress,
                ShippingAddress = sameAsShippingAddress ? new CAddress(billingAddress.CityTown, billingAddress.Country, billingAddress.PostalCode, billingAddress.Province, billingAddress.Street) :
                new CAddress(shippingAddress.CityTown, shippingAddress.Country, shippingAddress.PostalCode, shippingAddress.Province, shippingAddress.Street),
                ConvertToItemIssueVendor = convertToItemIssueVendor,
                IssueDate = issueDate,

                MultiCurrencySubTotal = multiCurrencySubTotal,
                MultiCurrencyTax = multiCurrencyTax,
                MultiCurrencyTotal = multiCurrencyTotal,
                MultiCurrencyOpenBalance = multiCurrencyTotal,
                MultiCurrencyTotalPaid = 0,
                IsItem = isItem,
                UseExchangeRate = useExchangeRate,
            };
        }
        public void IncreaseTotalPaid(decimal totalPaid)
        {
            TotalPaid += totalPaid;
        }

        public void IncreaseOpenbalance(decimal openBalance)
        {
            OpenBalance += openBalance;
        }

        public void IncreaseMultiCurrencyOpenBalance(decimal amount)
        {
            MultiCurrencyOpenBalance += amount;
        }

        public void IncreaseMultiCurrencyTotalPaid(decimal openBalance)
        {
            MultiCurrencyTotalPaid += openBalance;
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
            Guid vendorId,            
            bool sameAsShippingAddress,
            CAddress shippingAddress,
            CAddress billingAddress,
            decimal subTotal,
            decimal tax,
            decimal total,
            DateTime dueDate,
            DateTime? issueDate,
            bool convertToItemIssueVendor,
            decimal multiCurrencySubTotal,
            decimal multiCurrencyTax,
            decimal multiCurrencyTotal,
            Guid? itemReceipt,
            ReceiveFrom receiveFrom,
            bool isItem)
        {
            ReceiveFrom = receiveFrom;
            ItemReceiptId = itemReceipt;
            SubTotal = subTotal;
            Tax = tax;
            Total = total;
            TotalPaid = 0;
            MultiCurrencyTotalPaid = 0;
            OpenBalance = total;
            DueDate = dueDate;
            PaidStatus = PaidStatuse.Pending;
            ShipedStatus = DeliveryStatus.ShipPending;
            VendorId = vendorId;
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            SameAsShippingAddress = sameAsShippingAddress;
            BillingAddress.Update(billingAddress);
            ShippingAddress.Update(sameAsShippingAddress ? billingAddress : shippingAddress);
            IssueDate = issueDate;
            ConvertToItemIssueVendor = convertToItemIssueVendor;
            MultiCurrencyTotal = multiCurrencyTotal;
            MultiCurrencyTax = multiCurrencyTax;
            MultiCurrencySubTotal = multiCurrencySubTotal;
            MultiCurrencyOpenBalance = multiCurrencyTotal;
            IsItem = isItem;
        }

    }
}
