using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.AccountTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.CashFlowTemplates
{
    public enum CashFlowCategoryType
    {
        Operation = 1,
        Investment = 2,
        Finance = 3,
        CashTransfer = 4,
    }

    public static class CashFlowCategoryTypeExtensions
    {
        public static string GetName(this CashFlowCategoryType type)
        {

            var name = string.Empty;

            switch (type)
            {
                case CashFlowCategoryType.Operation : name = "Operation Activities"; break;
                case CashFlowCategoryType.Investment : name = "Investment Activities"; break;
                case CashFlowCategoryType.Finance : name = "Financing Activities"; break;
                case CashFlowCategoryType.CashTransfer : name = "Cash Transfer"; break;              
            }

            return name;
        }
    }




    [Table("CarlErpCashFlowCategories")]
    public class CashFlowCategory : AuditedEntity<Guid>, IMustHaveTenant
    {
        public const int MaxNameLength = 512;

        [Required]
        public int SortOrder { get; private set; } 

        [Required]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; }

        public string Description { get; set; }
        public int TenantId { get; set; }
        public CashFlowCategoryType Type { get; set; }
        public bool IsDefault { get; private set; }
        public void SetDefault(bool isDefault) => IsDefault = isDefault;

        public static CashFlowCategory Create(int tenantId, long? creatorUserId, CashFlowCategoryType type,  string name, int sortOrder, string description)
        {
            return new CashFlowCategory
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                TenantId = tenantId,
                Name = name,
                SortOrder = sortOrder,
                Description = description,
                Type = type

            };
        }

        public void Update(long? lastModifiedUserId, CashFlowCategoryType type, string name, int sortOrder, string description)

        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            Name = name;
            SortOrder = sortOrder;
            Description = description;
            Type = type;
        }
    }
}
