using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Vendors;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorarlERP.QCTests
{

    public enum LabTestType
    {
        Internal = 0,
        External = 1
    }

    public enum LabTestStatus
    {
        Pending = 0,
        Sent = 1,
        Received = 2
    }

    [Table("CarlErpLabTestRequests")]
    public class LabTestRequest : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public DateTime Date { get; set; }
        public long QCTestTemplateId { get; private set; }
        public QCTestTemplate QCTestTemplate { get; private set; }
        public Guid QCSampleId { get; private set; }
        public QCSample QCSample { get; private set; }
        public LabTestType Type { get; private set; }
        public Guid? LabId { get; private set; }
        public Vendor Lab { get; private set; }
        public LabTestStatus Status { get; set; }
        public string Remark { get; private set; }
        public void UpdateStatus(LabTestStatus status) => Status = status;

        public static LabTestRequest Create(int? tenantId, long creatorUserId, DateTime date, long qcTestTemplateId, Guid qcSampleId, LabTestType type, Guid? labId, string remark)
        {
            return new LabTestRequest()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                Date = date,
                QCTestTemplateId = qcTestTemplateId,
                QCSampleId = qcSampleId,
                Type = type,
                LabId = labId,
                Remark = remark,
                Status = LabTestStatus.Pending
            };
        }
        public void Update(long lastModifiedUserId, DateTime date, long qcTestTemplateId, Guid qcSampleId, LabTestType type, Guid? labId, string remark)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            Date = date;
            QCTestTemplateId = qcTestTemplateId;
            QCSampleId = qcSampleId;
            Type = type;
            LabId = labId;
            Remark = remark;
        }
    }
}
