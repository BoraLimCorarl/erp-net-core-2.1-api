using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.ChartOfAccounts;

namespace CorarlERP.PaymentMethods
{
    
    [Table("CarlErpPaymentMethodBases")]
    public class PaymentMethodBase : AuditedEntity<Guid>
    {
        public const int MaxNameLength = 512;

        [Required, MaxLength(MaxNameLength)]
        public string Name { get; private set; }
        public string Icon { get; private set; }

        public bool IsActive { get; private set; }
        public bool IsDefault { get; private set; }

        public static PaymentMethodBase Create(long creatorUserId, string name, string icon, bool isDefault = false)
        {
            return new PaymentMethodBase()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Icon = icon,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                IsDefault = isDefault,
                IsActive = true
            };
        }

        public void SetDefault(bool isDefault) { IsDefault = isDefault; }

        public void Enable(bool isEnable)
        {
            IsActive = isEnable;
        }

        public void Update(long lastModifiedUserId, string name, string icon, bool isDefault)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            Name = name;
            Icon = icon;
            IsDefault = isDefault;
        }

    }
}
