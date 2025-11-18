using Abp.Application.Editions;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Promotions;
using MailKit.Search;
using OfficeOpenXml.ConditionalFormatting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.PackageEditions
{
    [Table("CarlErpPackageEditionPromotions")]
    public class PackageEditionPromotion : AuditedEntity<Guid>
    { 
        public Guid PackageId { get; private set; }
        public Package Package { get; private set; }
        public int EditionId { get; private set; }
        public Edition Edition { get; private set; }
        public Guid PromotionId { get; private set; }
        public Promotion Promotion { get; private set; }


        public static PackageEditionPromotion Create(long userId, Guid packageId, int editionId, Guid promotionId)
        {
            return new PackageEditionPromotion
            {
                Id = Guid.NewGuid(),
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                PackageId = packageId,
                EditionId = editionId,
                PromotionId = promotionId
            };
        }

        public void Update(long userId, Guid packageId, int editionId, Guid promotionId)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            PackageId = packageId;
            EditionId = editionId;
            PromotionId = promotionId;
        }
    }
}
