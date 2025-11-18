using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.BatchNos;
using CorarlERP.Items;
using CorarlERP.Lots;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace CorarlERP.PhysicalCount
{
    [Table("CarlErpPhysicalCountItems")]
    public class PhysicalCountItem : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public int No { get; private set; }
        public Guid PhysicalCountId { get; private set; }
        public PhysicalCount PhysicalCount { get; private set; }

        public Guid ItemId { get; private set; }
        public Item Item { get; private set; }

        public long? LotId { get; private set; }
        public Lot Lot { get; private set; }
        public Guid? BatchNoId { get; private set; }
        public BatchNo BatchNo { get; private set; }
        public decimal CountQty { get; private set; }
        public decimal QtyOnHand { get; private set; }
        public decimal DiffQty { get; private set; }
        public string Description { get; private set; }
        public decimal UnitCost { get; private set; }

        public static PhysicalCountItem Create(
            int? tenantId,
            long? creatorUserId,
            Guid physicalCountId,
            int no,
            Guid itemId,
            long? lotId,
            Guid? batchNoId,
            decimal qtyOnHand,
            decimal countQty,
            decimal diffQty,
            decimal unitCost,
            string description)
        {
            return new PhysicalCountItem
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                PhysicalCountId = physicalCountId,
                No = no,
                ItemId = itemId,
                LotId = lotId,
                BatchNoId = batchNoId,
                QtyOnHand = qtyOnHand,
                CountQty = countQty,
                DiffQty = diffQty,
                UnitCost = unitCost,
                Description = description
            };
        }

        public void Update(
            long lastModifiedUserId,
            int no,
            Guid itemId,
            long? lotId,
            Guid? batchNoId,
            decimal qtyOnHand,
            decimal countQty,
            decimal diffQty,
            decimal unitCost,
            string description)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            No = no;
            ItemId = itemId;
            LotId = lotId;
            BatchNoId = BatchNoId;
            QtyOnHand = qtyOnHand;
            CountQty = countQty;
            DiffQty = diffQty;
            UnitCost = unitCost;
            Description = description;
        }

        public void UpdateCount(
            long? lastModifiedUserId,
            decimal countQty,
            decimal diffQty,
            decimal unitCost,
            string description)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            CountQty = countQty;
            DiffQty = diffQty;
            UnitCost = unitCost;
            Description = description;
        }
    }
}
