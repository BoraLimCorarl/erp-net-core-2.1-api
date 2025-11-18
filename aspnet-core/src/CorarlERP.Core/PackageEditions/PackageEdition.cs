using Abp.Application.Editions;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using OfficeOpenXml.ConditionalFormatting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Xml.Linq;

namespace CorarlERP.PackageEditions
{
    [Table("CarlErpPackageEditions")]
    public class PackageEdition : AuditedEntity<Guid>
    { 
        public Guid PackageId { get; private set; }
        public Package Package { get; private set; }
        public int EditionId { get; private set; }
        public Edition Edition { get; private set; }
        public int SortOrder { get; private set; }
        public decimal AnnualPrice { get; private set; }

        public static PackageEdition Create(long userId, Guid packageId, int editionId, int sortOrder, decimal annualPrice)
        {
            return new PackageEdition
            {
                Id = Guid.NewGuid(),
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                PackageId = packageId,
                EditionId = editionId,
                SortOrder = sortOrder,
                AnnualPrice = annualPrice
            };
        }

        public void Update(long userId, Guid packageId, int editionId, int sortOrder, decimal annualPrice)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            PackageId = packageId;
            EditionId = editionId;
            SortOrder = sortOrder;
            AnnualPrice = annualPrice;
        }
    }
}
