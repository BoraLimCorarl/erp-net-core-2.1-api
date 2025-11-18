using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Journals;
using CorarlERP.Locations;
using CorarlERP.ProductionProcesses;
using CorarlERP.Productions;
using CorarlERP.TransferOrders;
using CorarlERP.Vendors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ItemReceipts 
{
    [Table("CarlErpItemReceipts")]
    public class ItemReceipt : AuditedEntity<Guid>, IMayHaveTenant
    {
        public ReceiveFromStatus ReceiveFrom { get; private set; }
        public enum ReceiveFromStatus {
            None = 1,
            PO = 2,
            Bill = 3,
            TransferOrder = 4,
            PhysicalCount = 5,
            ProductionOrder = 6
        }

        public Guid? VendorId { get; private set; }
        public virtual Vendor Vendor { get; private set; }
        public InventoryTransactionType TransactionType { get; private set; }
             
        public bool SameAsShippingAddress { get; private set; }
        public CAddress BillingAddress { get; private set; }
        public CAddress ShippingAddress { get; private set; }

        public Guid? TransferOrderId { get; private set; }
        public virtual TransferOrder TransferOrder { get; private set; }

        public Guid? ProductionOrderId { get; private set; }
        public Production ProductionOrder { get; private set; }


        public long? ProductionProcessId { get; private set; }
        public ProductionProcess ProductionProcess { get; private set; }

        public Guid? PhysicalCountId { get; set; }
        public PhysicalCount.PhysicalCount PhysicalCount { get; set; }

        public decimal Total { get; private set; }
        public void SetTotal(decimal total) => Total = total;
        public int? TenantId { get; set; }
        
        public static ItemReceipt Create(int? tenantId, 
            long creatorUserId,
            ReceiveFromStatus receiveFrom,
            Guid? vendorId,         
           bool sameAsShippingAddress,
           CAddress shippingAddress , 
           CAddress billingAddress,
           decimal total,
           long? productionProcessId)
        {
            return new ItemReceipt()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ProductionProcessId = productionProcessId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,               
                Total = total,                
                VendorId =vendorId,
                ReceiveFrom = receiveFrom,
                BillingAddress = new CAddress(billingAddress.CityTown, billingAddress.Country, billingAddress.PostalCode, billingAddress.Province, billingAddress.Street),
                SameAsShippingAddress = sameAsShippingAddress,
                ShippingAddress = sameAsShippingAddress ? new CAddress(billingAddress.CityTown, billingAddress.Country, billingAddress.PostalCode, billingAddress.Province, billingAddress.Street) :
                new CAddress(shippingAddress.CityTown, shippingAddress.Country, shippingAddress.PostalCode, shippingAddress.Province, shippingAddress.Street),

            };
        }
        

        public void UpdateStatus(ReceiveFromStatus receiveFrom)
        {
            ReceiveFrom = receiveFrom;
        }

        public void UpdateTransferOrderId(Guid? id) {
            TransferOrderId = id;
        }
        public void UpdateProductionOrderId(Guid? id)
        {
            ProductionOrderId = id;
        }

        public void UpdatePhysicalCountId(Guid? id)
        {
            PhysicalCountId = id;
        }
        
        public void UpdateTotal(decimal total)
        {
            Total = total;
        }

        public void  UpdateVendor (Guid vendorId)
        {
            this.VendorId = vendorId;
           
        }

        public void Update(long lastModifiedUserId,
                ReceiveFromStatus receiveFrom,
                Guid? vendorId,              
                bool sameAsShippingAddress,
                CAddress shippingAddress,
                CAddress billingAddress,                 
                decimal total,
                long? productionProcessId)
        {           
            Total = total;
            ProductionProcessId = productionProcessId;
         
            VendorId = vendorId;
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


    }
}
