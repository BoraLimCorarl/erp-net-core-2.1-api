using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Addresses;
using CorarlERP.ItemReceipts;
using CorarlERP.Journals;
using CorarlERP.Locations;
using CorarlERP.Vendors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Bills
{
    [Table("CarlErpBills")]
    public class Bill : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public ReceiveFromStatus ReceiveFrom { get; private set; }
        public enum ReceiveFromStatus
        {
            None = 1,
            PO = 2,
            ItemReceipt = 3,            
        }

        public Guid? ItemReceiptId { get; private set; }
        public ItemReceipt ItemReceipt { get; private set; }

        public Guid VendorId { get; private set; }
        public Vendor Vendor { get;private set; }
    
        //public long LocationId { get; private set; }
        //public Location Location { get; private set; }

        public bool SameAsShippingAddress { get; private set; }
        public CAddress BillingAddress { get; private set; }
        public CAddress ShippingAddress { get; private set; }

        public decimal SubTotal { get; private set; }
        public decimal Tax { get; private set; }
        public decimal Total { get; private set; }

        public decimal MultiCurrencySubTotal { get; private set; }
        public decimal MultiCurrencyTax { get; private set; }
        public decimal MultiCurrencyTotal { get; private set; }


        public decimal OpenBalance { get; private set; }
        public decimal TotalPaid { get; set; }
        
        public decimal MultiCurrencyOpenBalance { get; private set; }
        public decimal MultiCurrencyTotalPaid { get; private set; }

        public DateTime DueDate { get; private set; }
        public DateTime ETA { get; private set; }
        public DeliveryStatus ReceivedStatus { get; private set; }

        public void SetDueDate(DateTime dueDate) => DueDate = dueDate;
        public void SetETA(DateTime etaDate) => ETA = etaDate;
        public void SetItemReceiptDate(DateTime? receiveDate) => ItemReceiptDate = receiveDate;
        
        
        public PaidStatuse PaidStatus { get; private set; }

        public bool ConvertToItemReceipt { get; private set; }

        public DateTime? ItemReceiptDate { get; private set;}

        public bool IsItem { get; private set; }
        public bool UseExchangeRate { get; private set; }

        public static Bill Create(
            int? tenantId, 
            long creatorUserId,
            ReceiveFromStatus status,
            DateTime dueDate,
            Guid vendorId, 
           // long locationId, 
            bool sameAsShippingAddress,
            CAddress shippingAddress, 
            CAddress billingAddress, 
            decimal subTotal, 
            decimal tax, 
            decimal total,
            Guid? itemReceiptId,DateTime eTA, bool convertToItemReceipt, DateTime? itemReceiptDate,
            decimal multiCurrencySubTotal,
            decimal multiCurrencyTax,
            decimal multiCurrencyTotal,
            bool isItemBill, 
            bool useExchangeRate)
        {
            return new Bill()
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
                VendorId = vendorId,
                ReceiveFrom = status,
                ItemReceiptId = itemReceiptId,
                PaidStatus = PaidStatuse.Pending,
                ReceivedStatus = DeliveryStatus.ReceivePending,
                BillingAddress = new CAddress(billingAddress.CityTown, billingAddress.Country, billingAddress.PostalCode, billingAddress.Province, billingAddress.Street),
                SameAsShippingAddress = sameAsShippingAddress,
                ShippingAddress = sameAsShippingAddress ? new CAddress(billingAddress.CityTown, billingAddress.Country, billingAddress.PostalCode, billingAddress.Province, billingAddress.Street) :
                new CAddress(shippingAddress.CityTown, shippingAddress.Country, shippingAddress.PostalCode, shippingAddress.Province, shippingAddress.Street),
                ETA = eTA,
                ConvertToItemReceipt = convertToItemReceipt,
                ItemReceiptDate = itemReceiptDate,

                MultiCurrencySubTotal = multiCurrencySubTotal,
                MultiCurrencyTax = multiCurrencyTax,
                MultiCurrencyTotal = multiCurrencyTotal,
                MultiCurrencyOpenBalance = multiCurrencyTotal,
                MultiCurrencyTotalPaid = 0,
                IsItem = isItemBill,
                UseExchangeRate = useExchangeRate,
            };
        }

        public void UpdateTotalPaid(decimal amount)
        {
            TotalPaid += amount;
        }

        public void UpdateOpenBalance(decimal amount)
        {
            OpenBalance += amount;
        }

       

        public void SetOpenBalance(decimal openBalance) { OpenBalance = openBalance; }
        public void SetMultiCurrencyOpenBalance(decimal openBalance) { MultiCurrencyOpenBalance = openBalance; }

        public void UpdateMultiCurrencyTotalPaid(decimal amount)
        {
            MultiCurrencyTotalPaid += amount;
        }

        public void UpdateMultiCurrencyOpenBalance(decimal amount)
        {
            MultiCurrencyOpenBalance += amount;
        }


        public void UpdateStatus(ReceiveFromStatus status)
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

        public void UpdateItemReceiptid(Guid? itemReceiptId)
        {
            ItemReceiptId = itemReceiptId;
            ItemReceipt = null;
        }

        public void SetSubTotal(decimal subTotal) { SubTotal = subTotal; }
        public void SetTax(decimal tax) { Tax = tax; }
        public void SetTotal(decimal total) { Total = total; }

        public void SetMultiCurrencySubTotal(decimal subTotal) { MultiCurrencySubTotal = subTotal; }
        public void SetMultiCurrencyTotal(decimal total) { MultiCurrencyTotal = total; }

        public void Update(
            long lastModifiedUserId,
            ReceiveFromStatus status,
            Guid vendorId, 
            DateTime dueDate,
          //  long locationId, 
            bool sameAsShippingAddress,
            CAddress shippingAddress, 
            CAddress billingAddress, 
            decimal subTotal, 
            decimal tax, 
            decimal total,
            Guid? itemReceiptId,
            DateTime eTA, bool conventToItemReceipt,DateTime? itemReceiptDate,
            decimal multiCurrencySubTotal,
            decimal multiCurrencyTax,
            decimal multiCurrencyTotal,
            bool isItemBill)
        {
            ItemReceiptId = itemReceiptId;
            DueDate = dueDate;
            SubTotal = subTotal;
            Tax = tax;
            Total = total;
            PaidStatus = PaidStatuse.Pending;
            ReceivedStatus = DeliveryStatus.ReceivePending;
            OpenBalance = total;
            MultiCurrencyOpenBalance = multiCurrencyTotal;
            TotalPaid = 0;
            MultiCurrencyTotalPaid = 0;
            VendorId = vendorId;           
            ReceiveFrom = status;
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            SameAsShippingAddress = sameAsShippingAddress;
            BillingAddress.Update(billingAddress);
            ShippingAddress.Update(sameAsShippingAddress ? billingAddress : shippingAddress);
            ETA = eTA;
            ConvertToItemReceipt = conventToItemReceipt;
            ItemReceiptDate = itemReceiptDate;
            MultiCurrencyTotal = multiCurrencyTotal;
            MultiCurrencyTax = multiCurrencyTax;
            MultiCurrencySubTotal = multiCurrencySubTotal;
            IsItem = IsItem;
        }

    }
}
