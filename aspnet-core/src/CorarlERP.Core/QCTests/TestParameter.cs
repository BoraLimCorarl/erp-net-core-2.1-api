using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorarlERP.QCTests
{
    public enum TestSources
    {
        QCInspection = 1,
        LabTest = 2,
        Both = 3
    }

    [Table("CarlErpTestParameters")]
    public class TestParameter : AuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxTestParameterNameLength = 512;
        public int? TenantId { get; set; }

        [Required]
        [MaxLength(MaxTestParameterNameLength)]
        public string Name { get; private set; }
        public TestSources TestSource { get; private set; }
        public string LimitReferenceNote { get; private set; }

        [Required]
        public bool IsActive { get; private set; }

        public static TestParameter Create(int? tenantId, long creatorUserId, string name, TestSources testSources, string limitReferenceNote)
        {
            return new TestParameter()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                IsActive = true,
                Name = name,
                TestSource = testSources,
                LimitReferenceNote = limitReferenceNote
            };
        }
        public void Enable(bool isEnable)
        {
            IsActive = isEnable;
        }
        public void Update(long lastModifiedUserId, string name, TestSources testSources, string limitReferenceNote)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            Name = name;
            TestSource = testSources;
            LimitReferenceNote = limitReferenceNote;
        }
    }
}
