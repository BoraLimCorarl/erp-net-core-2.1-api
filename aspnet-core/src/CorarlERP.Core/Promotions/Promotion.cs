using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.MultiTenancy;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Promotions
{

   

    [Table("CarlErpPromotions")]
    public class Promotion : AuditedEntity<Guid>
    {
        public string PromotionName { get; private set; }
        public PromotionType PromotionType { get; private set; }
        public decimal DiscountRate { get; private set; }
        public decimal ExtraMonth { get; private set; }
        public bool IsTrial { get; private set; }
        public bool IsActive { get; private set; }
        public void Enable(long userId, bool enable)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            IsActive = enable;
        }

        public static Promotion Create(long userId, PromotionType packagePromotionType, string promotionName, decimal discountRate, decimal extraMonth, bool isTrial)
        {
            return new Promotion
            {
                Id = Guid.NewGuid(),
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                PromotionType = packagePromotionType,
                PromotionName = promotionName,
                DiscountRate = discountRate,
                ExtraMonth = extraMonth,
                IsTrial = isTrial,
                IsActive = true
            };
        }

        public void Update(long userId, PromotionType packagePromotionType, string promotionName, decimal discountRate, decimal extraMonth, bool isTrial)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            PromotionType = packagePromotionType;
            PromotionName = promotionName;
            DiscountRate = discountRate;
            ExtraMonth = extraMonth;
            IsTrial = isTrial;
        }

    }




}
