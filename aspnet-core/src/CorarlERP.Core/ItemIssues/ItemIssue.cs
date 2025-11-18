using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Addresses;
using CorarlERP.Customers;
using CorarlERP.KitchenOrders;
using CorarlERP.Locations;
using CorarlERP.ProductionProcesses;
using CorarlERP.Productions;
using CorarlERP.TransactionTypes;
using CorarlERP.TransferOrders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ItemIssues
{
    [Table("CarlErpItemIssues")]
    public class ItemIssue : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public Guid? TransferOrderId { get; set; }
        public TransferOrder TransferOrder { get; set; }
        public Guid? ProductionOrderId { get; private set; }
        public Production ProductionOrder { get; private set; }
        public ReceiveFrom ReceiveFrom { get; private set; }
        public Guid? CustomerId { get; private set; }
        public Customer Customer { get; private set; }

    
        public long? ProductionProcessId { get; private set; }
        public ProductionProcess ProductionProcess { get; private set; }


        public bool SameAsShippingAddress { get; private set; }
        public bool ConvertToInvoice { get; private set; }
        public CAddress BillingAddress { get; private set; }
        public CAddress ShippingAddress { get; private set; }

        public decimal Total { get; private set; }
        public InventoryTransactionType TransactionType { get; set; }

        public Guid? PhysicalCountId { get; set; }
        public PhysicalCount.PhysicalCount PhysicalCount { get; set; }

        public TransactionType TransactionTypeSale { get; private set; } 
        public long? TransactionTypeSaleId { get; private set; }

        public Guid? KitchenOrderId { get; private set; }
        public KitchenOrder KitchenOrder { get; private set; }

        public static ItemIssue Create(int? tenantId, 
            long creatorUserId, 
            ReceiveFrom receiveFrom,
            Guid? customer,
            bool sameAsShippingAddress,
            CAddress shippingAddress, 
            CAddress billingAddress, 
            decimal total,
           long? productionProcessId,
           bool convertToInvoice = false)
        {
            return new ItemIssue()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                ProductionProcessId = productionProcessId,
                CreationTime = Clock.Now,
                Total = total,
                ConvertToInvoice = convertToInvoice,
                CustomerId = customer,
                ReceiveFrom = receiveFrom,
                BillingAddress = new CAddress(billingAddress.CityTown, billingAddress.Country,
                billingAddress.PostalCode, billingAddress.Province, billingAddress.Street),
                SameAsShippingAddress = sameAsShippingAddress,
                ShippingAddress = sameAsShippingAddress ?
                new CAddress(billingAddress.CityTown, billingAddress.Country,
                billingAddress.PostalCode, billingAddress.Province, billingAddress.Street) :
                new CAddress(shippingAddress.CityTown, shippingAddress.Country,
                shippingAddress.PostalCode, shippingAddress.Province, shippingAddress.Street),

            };
        }

        public void UpdateTransactionTypeId(long? transactionTypeSaleId)
        {
            TransactionTypeSaleId = transactionTypeSaleId;
        }
        public void UpdateStatus(ReceiveFrom receiveFrom)
        {
            ReceiveFrom = receiveFrom;
        }
        public void UpdateTransactionType(InventoryTransactionType type)
        {
            TransactionType = type;
        }

        public void SetKitchenOrder (Guid? kitchenOrderId)
        {
            this.KitchenOrderId = kitchenOrderId;
        }
        public void Update(long lastModifiedUserId,
                ReceiveFrom receiveFrom,
                Guid? customerId,
               
                bool sameAsShippingAddress,
                CAddress shippingAddress,
                CAddress billingAddress,
                decimal total,
                long? productionProcessId,
                bool convertToInvoice = false)
        {
            Total = total;
            ProductionProcessId = productionProcessId;
            ConvertToInvoice = convertToInvoice;
            CustomerId = customerId;
            ReceiveFrom = receiveFrom;
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            SameAsShippingAddress = sameAsShippingAddress;
            BillingAddress.Update(billingAddress);
            ShippingAddress.Update(sameAsShippingAddress ? billingAddress : shippingAddress);
        }


        public void UpdateTotal(decimal total)
        {
            Total = total;
        }

        public void UpdateTransferOrderId(Guid? transferOrderId)
        {
            TransferOrderId = transferOrderId;
        }

        public void UpdateProductionOrderId (Guid? productionOrderId)
        {
            ProductionOrderId = productionOrderId;
        }
        public void UpdatePhysicalCountId(Guid? physicalCountId)
        {
            PhysicalCountId = physicalCountId;
        }

        public void UpdateCustomer( Guid customerId)
        {
            CustomerId = customerId;           
        }
    }
}
