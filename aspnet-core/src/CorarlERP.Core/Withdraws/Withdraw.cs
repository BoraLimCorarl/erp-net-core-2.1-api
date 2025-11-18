using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Customers;
using CorarlERP.Locations;
using CorarlERP.Vendors;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorarlERP.Withdraws
{
    [Table("CarlErpWithdraws")]
    public class Withdraw : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public Guid? VendorId { get; private set; }
        public Vendor Vendor { get; private set; }


        public Guid? CustomerId { get; private set; }
        public Customer Customer { get; private set; }

        public decimal Total { get; private set; }
        public Guid? BankTransferId { get; private set; }      
        public static Withdraw Create(int? tenantId, long creatorUserId, Guid? vendorId, Guid? customerId, decimal total)
        {
            return new Withdraw()
            {
                CustomerId = customerId,
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                Total = total,
                VendorId = vendorId,
            };
        }

        public void UpdateBankTransferId(Guid id)
        {
            BankTransferId = id;
        }

        public void Update(long lastModifiedUserId, decimal total, Guid? vendorId, Guid? customerId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            Total = total;
            VendorId = vendorId;
            CustomerId = customerId;
          
        }
    }
}
