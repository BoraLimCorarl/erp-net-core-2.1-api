using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.SubItems
{
    [Table("CarlErpSubItems")]
    public class SubItem : AuditedEntity<Guid>, IMayHaveTenant
    {
        
        public int? TenantId { get; set; }
        public bool IsActive { get; private set; }
        public decimal Cost{ get; private set; } 
        public decimal Quantity { get; private set; }
        public decimal Total { get; private set; }
        
        public Guid ItemId { get; private set; }//parent Item from create Item
        public Item Item { get; private set; }

        public Guid? ParentSubItemId { get; private set; }// parent Id of subtem
        public Item ParentSubItem { get; private set; }


        //private static SubItem CreateSub(int? tenantId, long? creatorUserId, decimal quantity, decimal total, decimal cost)
        //{
        //    return new SubItem
        //    {
        //        Id = Guid.NewGuid(),
        //        CreatorUserId = creatorUserId,
        //        TenantId = tenantId,
        //        Cost = cost,
        //        IsActive = true,
        //        Total = total,
        //        Quantity = quantity
        //    };
        //}

        //public static SubItem Create(int? tenantId, long? creatorUserId, decimal quantity,decimal total,decimal cost, Guid parentSubItemId, Guid itemId)
        //{
        //    var subItem = CreateSub(tenantId, creatorUserId, quantity, total, cost);
        //    subItem.ItemId = itemId;
        //    subItem.ParentSubItemId = parentSubItemId;
        //    return subItem;

        //}
        

        //public static SubItem Create(int? tenantId, long? creatorUserId, decimal quantity, decimal total, decimal cost, Item parentSubItem, Guid itemId)
        //{
        //    var subItem = CreateSub(tenantId, creatorUserId, quantity, total, cost);
        //    subItem.ItemId = itemId;
        //    subItem.ParentSubItem = parentSubItem;
        //    return subItem;
        //}

        //public void Update(long lastModifiedUserId, decimal quantity,decimal cost, decimal total, Guid itemId)
        //{
        //    LastModifierUserId = lastModifiedUserId;
        //    LastModificationTime = Clock.Now;
        //    Total = total;
        //    Cost = cost;
        //    Quantity = quantity;          
        //    IsActive = true;
        //    ItemId = itemId;
        //}
        public void Enable(bool isEnable)
        {
            IsActive = isEnable;
        }

    }
}
