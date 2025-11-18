using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Vendors;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorarlERP.QCTests
{   

    [Table("CarlErpLabTestResults")]
    public class LabTestResult : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public Guid LabTestRequestId { get; private set; }
        public string ReferenceNo { get; private set; } 
        public LabTestRequest LabTestRequest { get; private set; }
        public Guid? LabId { get; private set; }
        public Vendor Lab { get; private set; }
        public bool DetailEntry { get; private set; } // Default to false, can be changed if needed

        public bool FinalPassFail { get; private set; }
        public DateTime ResultDate { get; private set; }

        public static LabTestResult Create(int? tenantId, long creatorUserId, DateTime resultDate, string ReferenceNo, Guid labTestRequestId, Guid? labId, bool detailEntry, bool finalPassFail)
        {
            return new LabTestResult()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                ReferenceNo = ReferenceNo,
                LabTestRequestId = labTestRequestId,
                LabId = labId,
                ResultDate = resultDate,
                DetailEntry = detailEntry,
                FinalPassFail = finalPassFail
            };
        }
        public void Update(long lastModifiedUserId, DateTime resultDate, string referenceNo, Guid labTestRequestId, Guid? labId, bool detailEntry, bool finalPassFail)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            LabTestRequestId = labTestRequestId;
            LabId = labId;
            ReferenceNo = referenceNo;
            ResultDate = resultDate;
            DetailEntry = detailEntry;
            FinalPassFail = finalPassFail;
        }
    }
}
