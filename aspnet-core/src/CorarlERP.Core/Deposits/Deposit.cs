using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Customers;
using CorarlERP.Locations;
using CorarlERP.Vendors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Deposits
{
    [Table("CarlErpDeposits")]
    public class Deposit : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public Guid? ReceiveFromVendorId { get; private set; }
        public Vendor Vendor { get; private set; }

        public Guid? ReceiveFromCustomerId { get; private set; }
        public Customer Customer { get; private set; }
        public decimal Total { get; private set; }
        public Guid? BankTransferId { get; private set; }
        
        public static Deposit Create(
            int? tenantId,
            long creatorUserId,
            Guid? receiveFromVendorId,
            Guid? receiveFromCustomerId,
            decimal total)
        {
            return new Deposit()
            {
                ReceiveFromCustomerId = receiveFromCustomerId,
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                Total = total,
                ReceiveFromVendorId = receiveFromVendorId,               
            };
        }

        public void Update(
            long lastModifiedUserId,
            Guid? receiveFromVendorId,
            Guid? receiveFromCustomerId,
            decimal total)
        {            
            Total = total;
            ReceiveFromCustomerId = receiveFromCustomerId;
            ReceiveFromVendorId = receiveFromVendorId;
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
        }

        public void UpdateBankTransferId(Guid id)
        {
            BankTransferId = id;
        }
    }
}
