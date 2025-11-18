using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.BatchNos;
using CorarlERP.Lots;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.InventoryCostCloses
{
    [Table("CarlErpInventoryCostCloseItemBatchNos")]
    public class InventoryCostCloseItemBatchNo : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set ; }

        public long LotId { get; private set; }
        public Lot Lot { get; private set; } 

        public Guid BatchNoId { get; private set; }
        public BatchNo BatchNo { get; private set; }

        public decimal Qty { get; private set; }

        public InventoryCostClose InventoryCostClose { get; private set; }
        public Guid InventoryCostCloseId { get; private set; }

        public static InventoryCostCloseItemBatchNo Create(int? tenantId, long creatorUserId, long lotId, Guid batchNoId, decimal qty,Guid inventoryCostCloseId )
        {
            return new InventoryCostCloseItemBatchNo()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                InventoryCostCloseId = inventoryCostCloseId,
                LotId = lotId,
                BatchNoId = batchNoId,
                Qty = qty,
            };
        }
        public void Upudate(long lastModifiedUserId, long lotId, Guid batchNoId, decimal qty, Guid inventoryCostCloseId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            LotId = lotId;
            BatchNoId = batchNoId;
            Qty = qty;
            InventoryCostCloseId = inventoryCostCloseId;
        }
    }

}

