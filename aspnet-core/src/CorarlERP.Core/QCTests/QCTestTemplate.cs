using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorarlERP.QCTests
{

    [Table("CarlErpQCTestTemplates")]
    public class QCTestTemplate : AuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxQCTestTemplateNameLength = 512;
        public int? TenantId { get; set; }

        [Required]
        [MaxLength(MaxQCTestTemplateNameLength)]
        public string Name { get; private set; }
        public TestSources TestSource { get; private set; }
        public bool DetailEntryRequired { get; private set; }

        [Required]
        public bool IsActive { get; private set; }

        public static QCTestTemplate Create(int? tenantId, long creatorUserId, string name, TestSources testSources, bool detailEntryRequied)
        {
            return new QCTestTemplate()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                IsActive = true,
                Name = name,
                TestSource = testSources,
                DetailEntryRequired = detailEntryRequied
            };
        }
        public void Enable(bool isEnable)
        {
            IsActive = isEnable;
        }
        public void Update(long lastModifiedUserId, string name, TestSources testSources, bool detailEntryRequied)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            Name = name;
            TestSource = testSources;
            DetailEntryRequired = detailEntryRequied;
        }
    }
}
