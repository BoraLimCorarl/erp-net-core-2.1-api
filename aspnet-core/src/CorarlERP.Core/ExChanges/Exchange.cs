using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;

namespace CorarlERP.ExChanges
{

    [Table("CarlErpExchanges")]
    public class Exchange : AuditedEntity<Guid>, IMayHaveTenant
    {

        public DateTime FromDate { get; private set; }

        public DateTime ToDate { get; private set; }

        public int? TenantId { get; set; }


        public static Exchange Create(int? tenantId, long creatorUserId, DateTime fromDate ,DateTime todate)
        {
            return new Exchange()
            {
                Id = new Guid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                FromDate = fromDate,
                ToDate = todate,

            };
        }

        public void Update(long lastModifiedUserId, DateTime fromDate, DateTime todate)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            FromDate = fromDate;
            ToDate = todate;
        }

    }
}
