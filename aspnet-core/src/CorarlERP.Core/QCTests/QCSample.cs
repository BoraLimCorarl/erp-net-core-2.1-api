using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Items;
using CorarlERP.Locations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorarlERP.QCTests
{

    [Table("CarlErpQCSamples")]
    public class QCSample : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [Required]
        public string SampleId { get; private set; }
        public string SourceDoc { get; private set; }
        public DateTime SampleDate { get; private set; }
        public Guid ItemId { get; private set; }
        public Item Item { get; private set; }
        public long LocationId { get; private set; }
        public Location Location { get; private set; }
        public string Remark { get; private set; }


        public static QCSample Create(int? tenantId, long creatorUserId, string sampleId, string sourcesDoc, DateTime sampleDate, Guid itemId, long locationId, string remark)
        {
            return new QCSample()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                SampleDate = sampleDate,
                SampleId = sampleId,
                SourceDoc = sourcesDoc,
                ItemId = itemId,
                LocationId = locationId,
                Remark = remark
            };
        }
        public void Update(long lastModifiedUserId, string sampleId, string sourcesDoc, DateTime sampleDate, Guid itemId, long locationId, string remark)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            SampleId = sampleId;
            SourceDoc = sourcesDoc;
            SampleDate = sampleDate;
            ItemId = itemId;
            LocationId = locationId;
            Remark = remark;
        }
    }
}
