using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Invoices;
using CorarlERP.ItemIssues;
using CorarlERP.Items;
using CorarlERP.SaleOrders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.DeliverySchedules
{
    [Table("CarlErpDeliverySheduleItems")]
    public class DeliveryScheduleItem : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public Item Item { get; private set; }
        public Guid ItemId { get; private set; }
        public decimal Qty { get; private set; }
        public string Description { get; private set; }
        public DeliverySchedule DeliverySchedule { get; private set; }
        public Guid DeliveryScheduleId { get; private set; }

        public SaleOrderItem  SaleOrderItem { get; private set; }
        public Guid? SaleOrderItemId { get; private set; }
        public  ICollection<InvoiceItem> InvoiceItems { get; private set; }
        public  ICollection<ItemIssueItem> ItemIssueItems { get; private set; }
        public static DeliveryScheduleItem Create(int? tenantId, long creatorUserId, Guid deliveryScheduleId, string description, decimal qty,Guid? saleOrderItemId, Guid itemId
         )
        {
            return new DeliveryScheduleItem()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                DeliveryScheduleId = deliveryScheduleId,
                Description = description,
                Qty = qty,
                SaleOrderItemId = saleOrderItemId,
                ItemId = itemId,

            };
        }

        public void Update(long lastModifiedUserId, Guid deliveryScheduleId, string description, decimal qty,Guid? saleOrderItemId, Guid itemId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            DeliveryScheduleId = deliveryScheduleId;
            Description = description;
            Qty = qty;
            SaleOrderItemId = saleOrderItemId;
            ItemId = itemId;

        }
    }
}
