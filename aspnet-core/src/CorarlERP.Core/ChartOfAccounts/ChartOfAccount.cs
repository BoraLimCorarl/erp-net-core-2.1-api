using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Taxes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.ChartOfAccounts
{
    [Table("CarlErpChartOfAccounts")]
    public class ChartOfAccount : AuditedEntity<Guid>, IMayHaveTenant
    {
        public const int MaxAccountCodeLength = 16;
        public const int MaxAccountNameLength = 512;

        #region Properties
        public int? TenantId { get; set; }

        [Required]
        [MaxLength(MaxAccountCodeLength)]
        public string AccountCode { get; private set; }

        [Required]
        [MaxLength(MaxAccountNameLength)]
        public string AccountName { get; private set; }
        
        public string Description { get; private set; }

        public long TaxId { get; private set; }
        public Tax Tax { get; private set; }

        public long AccountTypeId { get; private set; }
        public AccountType AccountType { get; private set; }

        public Guid? ParentAccountId { get; private set; }
        public ChartOfAccount ParentAccount { get; private set; }

        [Required]
        public bool IsActive { get; private set; }

        public SubAccountType? SubAccountType { get; private set; }
        public void SetSubAccountingType(SubAccountType? subAccountType) => SubAccountType = subAccountType;

        #endregion  

        public static ChartOfAccount Create(int? tenantId, long creatorUserId, 
                                            string accountCode, string accountName, 
                                            string description, 
                                            long accountTypeId,
                                            Guid? parentAccountId, 
                                            long taxId)
        {
            return new ChartOfAccount()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                AccountCode = accountCode,
                AccountName = accountName,
                Description = description,
                AccountTypeId = accountTypeId,
                ParentAccountId = parentAccountId,
                TaxId = taxId,
                IsActive = true
            };
        }

        public void Update(long lastModifiedUserId, string accountCode, string accountName, 
                           string description, long accountTypeId, Guid? parentAcountId, long taxId)
        {
            this.LastModifierUserId = lastModifiedUserId;
            this.LastModificationTime = Clock.Now;
            this.AccountCode = accountCode;
            this.AccountName = accountName;
            this.Description = description;
            this.ParentAccountId = parentAcountId;
            this.AccountTypeId = accountTypeId;
            this.TaxId = taxId;
        }

        public void Enable(bool isEnable)
        {
            this.IsActive = isEnable;
        }

    }
}
