using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorarlERP.QCTests
{   

    [Table("CarlErpLabTestResultDetails")]
    public class LabTestResultDetail : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public Guid LabTestResultId { get; private set; }
        public LabTestResult LabTestResult { get; private set; }

        public long TestParameterId { get; private set; }
        public TestParameter TestParameter { get; private set; }
        public string LimitReferenceNote { get; private set; }
        public string ActualValueNote { get; private set; }
        public bool PassFail { get; private set; }

        public static LabTestResultDetail Create(int? tenantId, long creatorUserId, Guid labTestResultId, long testParameterId, string limitReferenceNote, string actualValueNote, bool passFail)
        {
            return new LabTestResultDetail()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                LabTestResultId = labTestResultId,
                TestParameterId = testParameterId,
                LimitReferenceNote = limitReferenceNote,
                ActualValueNote = actualValueNote,
                PassFail = passFail
            };
        }

        public void Update(long lastModifiedUserId, long testParameterId, string limitReferenceNote, string actualValueNote, bool passFail)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            TestParameterId = testParameterId;
            LimitReferenceNote = limitReferenceNote;
            ActualValueNote = actualValueNote;
            PassFail = passFail;
        }
    }
}
