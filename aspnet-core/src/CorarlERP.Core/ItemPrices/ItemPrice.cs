using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.CustomerTypes;
using CorarlERP.Locations;
using CorarlERP.TransactionTypes;

namespace CorarlERP.ItemPrices
{
    [Table("CarlErpItemPrices")]
    public class ItemPrice : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public TransactionType TransactionTypeSale { get; private set; }
        public long? TransactionTypeSaleId { get; private set; }

        public Location Location { get; private set; }
        public long? LocationId { get; private set; }

        public long? CustomerTypeId { get; private set; }
        public CustomerType CustomerType { get; private set; }

        public static ItemPrice Create(int? tenantId, long creatorUserId, long? locationId, long? transactionSaleTypeId, long? customerTypeId)
        {
            return new ItemPrice()
            {
                Id = new Guid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                LocationId = locationId,
                TransactionTypeSaleId = transactionSaleTypeId,
                CustomerTypeId = customerTypeId,     
            };
        }


        public void Update(long lastModifiedUserId, long? locationId, long? transactionSaleTypeId, long? customerTypeId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            LocationId = locationId;
            TransactionTypeSaleId = transactionSaleTypeId;
            CustomerTypeId = customerTypeId;
        }

    }
}
