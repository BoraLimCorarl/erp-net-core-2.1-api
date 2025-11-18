using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.AccountCycles
{
    [Table("CarlErpAccountCycles")]
    public class AccountCycle : AuditedEntity<long>, IMayHaveTenant
    {
        public int RoundingDigit { get; set; } // For rounding general of total
        public int RoundingDigitUnitCost { get; set; } // For rounding only unit cost of item
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? TenantId { get; set; }
        public bool IsActive { get; private set; }
        public string Remark { get; private set; }

        public static AccountCycle Create(
            int tenantId, 
            long? creatorUserId, 
            DateTime startDate,
            DateTime? endDate,
            int roundingDigit,
            int roundingDigitUnitCost)
        {
            return new AccountCycle()
            {

                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                TenantId = tenantId,
                EndDate = endDate,
                StartDate = startDate,
                IsActive = true,
                RoundingDigit = roundingDigit,
                RoundingDigitUnitCost = roundingDigitUnitCost
            };
        }
        public void Enable(bool isEnable)
        {
            this.IsActive = isEnable;
        }

        public void UpdateEndate(DateTime? endDate)
        {
            EndDate = endDate;
        }
          
        public void UpdateRemark(string remark)
        {
            Remark = remark;
        }
        public void Update(long lastModifiedUserId, DateTime startDate, DateTime? endDate, int roundDigit, int roundDigitUnitCost)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            StartDate = startDate;
            EndDate = endDate;
            IsActive = true;
            RoundingDigit = roundDigit;
            RoundingDigitUnitCost = roundDigitUnitCost;
        }
    }
}
