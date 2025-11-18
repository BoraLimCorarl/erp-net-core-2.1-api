using Abp.Application.Editions;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using MailKit.Search;
using OfficeOpenXml.ConditionalFormatting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.PackageEditions
{
    [Table("CarlErpPackageEditionFeatures")]
    public class PackageEditionFeature : AuditedEntity<Guid>
    { 
        public Guid PackageId { get; private set; }
        public Package Package { get; private set; }
        public int EditionId { get; private set; }
        public Edition Edition { get; private set; }
        public Guid FeatureId { get; private set; }
        public Feature Feature { get; private set; }

        [Required]
        [MaxLength(CorarlERPConsts.MaxNameLength)]
        public string Value { get; private set; }

        public static PackageEditionFeature Create(long userId, Guid packageId, int editionId, Guid featureId, string value)
        {
            return new PackageEditionFeature
            {
                Id = Guid.NewGuid(),
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                PackageId = packageId,
                EditionId = editionId,
                FeatureId = featureId,
                Value = value,
            };
        }

        public void Update(long userId, Guid packageId, int editionId, Guid featureId, string value)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            PackageId = packageId;
            EditionId = editionId;
            FeatureId = featureId;
            Value = value;
        }
    }
}
