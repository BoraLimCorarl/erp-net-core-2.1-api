using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Addresses;
using CorarlERP.CustomerCredits;
using CorarlERP.Customers;
using CorarlERP.ItemIssues;
using CorarlERP.Locations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ItemReceiptCustomerCredits
{
    [Table("CarlErpItemReceiptCustomerCredit")]
    public class ItemReceiptCustomerCredit : AuditedEntity<Guid>, IMayHaveTenant
    {

        public ReceiveFrom ReceiveFrom { get; private set; }
       

        public Guid? CustomerCreditId { get; private set; }
        public CustomerCredit CustomerCredit { get; private set; }

        public Guid CustomerId { get; private set; }
        public virtual Customer Customer { get; private set; }
        public InventoryTransactionType TransactionType { get; private set; }
     
        public bool SameAsShippingAddress { get; private set; }
        public CAddress BillingAddress { get; private set; }
        public CAddress ShippingAddress { get; private set; }


        public decimal Total { get; private set; }
        public int? TenantId { get; set; }

        public Guid? ItemIssueSaleId { get; private set; }
        public ItemIssue ItemIssueSale { get; private set; }
        public static ItemReceiptCustomerCredit Create(
            int? tenantId, 
            long creatorUserId, 
            Guid? customerCreditId,
            ReceiveFrom receiveFrom,
            Guid customerId,           
            bool sameAsShippingAddress,
            CAddress shippingAddress, 
            CAddress billingAddress, 
            decimal total,
            Guid? itemIssueSaleId)
        {
            return new ItemReceiptCustomerCredit()
            {
                ItemIssueSaleId = itemIssueSaleId,
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CustomerCreditId = customerCreditId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                Total = total,
                
                CustomerId = customerId,
                ReceiveFrom = receiveFrom,
                BillingAddress = new CAddress(billingAddress.CityTown, billingAddress.Country, billingAddress.PostalCode, billingAddress.Province, billingAddress.Street),
                SameAsShippingAddress = sameAsShippingAddress,
                ShippingAddress = sameAsShippingAddress ? new CAddress(billingAddress.CityTown, billingAddress.Country, billingAddress.PostalCode, billingAddress.Province, billingAddress.Street) :
                new CAddress(shippingAddress.CityTown, shippingAddress.Country, shippingAddress.PostalCode, shippingAddress.Province, shippingAddress.Street),

            };
        }

        public void UpdateTotal(decimal total)
        {
            Total = total;
        }
        public void UpdateStatus(ReceiveFrom receiveFrom)
        {
            ReceiveFrom = receiveFrom;
        }

        public void Update(long lastModifiedUserId,
                ReceiveFrom receiveFrom,
                Guid customerId,             
                bool sameAsShippingAddress,
                CAddress shippingAddress,
                CAddress billingAddress,
                Guid? customerCreditId,
                decimal total,
                Guid? itemIssueSaleId)
        {
            ItemIssueSaleId = itemIssueSaleId;
            CustomerCreditId = customerCreditId;
            Total = total;           
            CustomerId = customerId;
            ReceiveFrom = receiveFrom;
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
        public void UpdateCustomerCredit (Guid? customerCrditId)
        {
            CustomerCreditId = customerCrditId;
        }

    }
}
