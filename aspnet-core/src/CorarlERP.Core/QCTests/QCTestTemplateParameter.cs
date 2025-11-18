using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorarlERP.QCTests
{

    [Table("CarlErpQCTestTemplateParameters")]
    public class QCTestTemplateParameter : AuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public long QCTestTemplateId { get; set; }
        public QCTestTemplate QCTestTemplate { get; set; }
        public string LimitReferenceNoteOverride { get; set; }

        public static QCTestTemplateParameter Create(int? tenantId, long creatorUserId, long qcTestTemplateId, string limitReferenceNoteOverride)
        {
            return new QCTestTemplateParameter()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                QCTestTemplateId = qcTestTemplateId,
                LimitReferenceNoteOverride = limitReferenceNoteOverride
            };
        }
        
        public void Update(long lastModifiedUserId, string limitReferenceNoteOverride)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            LimitReferenceNoteOverride = limitReferenceNoteOverride;
        }
    }
}
