using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Locations;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.PaymentMethods
{
    [Table("CarlErpPaymentMethods")]
    public class PaymentMethod : BaseAuditedEntity<Guid>
    {   
        public Guid PaymentMethodId { get; private set; }
        public PaymentMethodBase PaymentMethodBase { get; private set; }
        public Guid AccountId { get; private set; }
        public ChartOfAccount Account { get; private set; }

        public Member Member { get; set; }
        public void SetMember(Member member) => Member = member;

        public bool IsActive { get; private set; }

        public static PaymentMethod Create(int? tenantId, long creatorUserId, Guid paymentMethodId, Guid accountId)
        {
            return new PaymentMethod()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                PaymentMethodId = paymentMethodId,
                AccountId = accountId,
                IsActive = true,
                Member = Member.All
            };
        }

        public void Enable(bool isEnable)
        {
            IsActive = isEnable;
        }

        public void Update(long lastModifiedUserId, Guid paymentMethodId, Guid accountId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            PaymentMethodId = paymentMethodId;
            AccountId = accountId;
        }

    }
   
}
