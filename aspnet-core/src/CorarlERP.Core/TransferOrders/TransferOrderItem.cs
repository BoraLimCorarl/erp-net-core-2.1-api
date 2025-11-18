using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CorarlERP.BatchNos;
using CorarlERP.Items;
using CorarlERP.Lots;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.TransferOrders
{
    [Table("CarlErpTransferOrderItems")]
    public class TransferOrderItem : AuditedEntity<Guid>, IMayHaveTenant
    {

        public int? TenantId { get; set; }

        public Guid TransferOrderId { get; private set; }
        public TransferOrder TransferOrder { get; private set; }

        public Guid ItemId { get; private set; }
        public Item Item { get; private set; }
        
        public string Description { get; private set; }

        public decimal Qty { get; private set; }

        public Lot FromLot { get; private set; }
        public long FromLotId { get; private set; }

        public Lot ToLot { get; private set; }
        public long ToLotId { get; private set; }

        private static TransferOrderItem Create(int? tenantId, long? creatorUserId, Guid itemId, string description, decimal qty)
        {
            return new TransferOrderItem
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                TenantId = tenantId,
                Description = description,
                Qty = qty,
                ItemId = itemId
            };
        }


        public static TransferOrderItem Create(
            int? tenantId,
            long? creatorUserId,
            TransferOrder transferOrder,
            Guid itemId,
            string description,
            decimal qty)
        {
            var result = Create(tenantId, creatorUserId, itemId, description, qty);
            result.TransferOrder = transferOrder;
            return result;

        }

        public void UpdateFromLot(long fromLotId)
        {
            FromLotId = fromLotId;
        }

        public void UpdatToLot(long toLotId)
        {
            ToLotId = toLotId;
        }
        
        public static TransferOrderItem Create(
            int? tenantId,
            long? creatorUserId,
            Guid transferOrderId,
            Guid itemId,
            string description,
            decimal qty)
        {
            var result = Create(tenantId, creatorUserId, itemId, description, qty);
            result.TransferOrderId = transferOrderId;
            return result;

        }

        public void Update(
            long lastModifiedUserId,
            Guid itemId,
            string description,
            decimal qty)
        {
            LastModifierUserId = lastModifiedUserId;
            Description = description;
            Qty = qty;
            ItemId = itemId;
        }
    }
}
