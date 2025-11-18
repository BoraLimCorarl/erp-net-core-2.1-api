using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Addresses;
using CorarlERP.ItemReceipts;
using CorarlERP.Vendors;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ItemIssueVendorCredits
{
    [Table("CarlErpItemIssueVendorCredit")]
    public class ItemIssueVendorCredit : AuditedEntity<Guid>, IMayHaveTenant
    {

        public ReceiveFrom ReceiveFrom { get; private set; }
       
        public Guid? VendorCreditId { get; private set; }
        public VendorCredit.VendorCredit VendorCredit { get; private set; }

        public Guid VendorId { get; private set; }
        public Vendor Vendor { get; private set; }
        public InventoryTransactionType TransactionType { get; private set; }
     

        public bool SameAsShippingAddress { get; private set; }
        public CAddress BillingAddress { get; private set; }
        public CAddress ShippingAddress { get; private set; }


        public decimal Total { get; private set; }
        public int? TenantId { get; set; }

        public Guid? ItemReceiptPurchaseId { get; private set; }
        public ItemReceipt ItemReceiptPurchase { get; private set; }

        public static ItemIssueVendorCredit Create(
            int? tenantId,
            long creatorUserId,
            Guid? vendorCreditId,
            ReceiveFrom receiveFrom,
            Guid vendorId,         
            bool sameAsShippingAddress,
            CAddress shippingAddress,
            CAddress billingAddress,
            decimal total,
            Guid? itemReceiptPurchaseId)
        {
            return new ItemIssueVendorCredit()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                VendorCreditId = vendorCreditId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                Total = total,              
                VendorId = vendorId,
                ReceiveFrom = receiveFrom,
                ItemReceiptPurchaseId = itemReceiptPurchaseId,
                BillingAddress = new CAddress(billingAddress.CityTown, billingAddress.Country, billingAddress.PostalCode, billingAddress.Province, billingAddress.Street),
                SameAsShippingAddress = sameAsShippingAddress,
                ShippingAddress = sameAsShippingAddress ? new CAddress(billingAddress.CityTown, billingAddress.Country, billingAddress.PostalCode, billingAddress.Province, billingAddress.Street) :
                new CAddress(shippingAddress.CityTown, shippingAddress.Country, shippingAddress.PostalCode, shippingAddress.Province, shippingAddress.Street),

            };
        }


        public void UpdateStatus(ReceiveFrom receiveFrom)
        {
            ReceiveFrom = receiveFrom;
        }

        public void Update(long lastModifiedUserId,
                ReceiveFrom receiveFrom,
                Guid vendorId,               
                bool sameAsShippingAddress,
                CAddress shippingAddress,
                CAddress billingAddress,
                Guid? vendorCreditId,
                decimal total,
                Guid? itemReceiptPurchaseId)
        {
            VendorCreditId = vendorCreditId;
            Total = total;            
            VendorId = vendorId;
            ReceiveFrom = receiveFrom;
            ItemReceiptPurchaseId = itemReceiptPurchaseId;
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            SameAsShippingAddress = sameAsShippingAddress;
            BillingAddress.Update(billingAddress);
            ShippingAddress.Update(sameAsShippingAddress ? billingAddress : shippingAddress);
        }

        public void UpdateTransactionType(InventoryTransactionType type)
        {
            TransactionType = type;
        }

        public void UpdateVendorCreditId (Guid? vendorcreditId)
        {
            VendorCreditId = vendorcreditId;
        }

        public void UpdateTotal(decimal total)
        {
            Total = total;
        }
    }
}
