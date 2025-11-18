using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Taxes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ChartOfAccounts
{
    [Table("CarlErpAccountTypes")]
    public class AccountType : AuditedEntity<long>
    {
        public const int MaxAccountTypeNameLength = 512;

        #region Properties
      
      
        [Required]
        [MaxLength(MaxAccountTypeNameLength)]
        public string AccountTypeName { get; private set; }

        public TypeOfAccount Type { get; private set; }

        public string Description { get; private set; }

        public bool IsActive { get; private set; }

    
        #endregion  

        public static AccountType Create(long creatorUserId, 
                                         string accountTypeName,
                                         TypeOfAccount type,
                                         string description)
        {
            return new AccountType()
            {
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                AccountTypeName = accountTypeName,
                Type = type,
                Description = description,
                IsActive = true
            };
        }

        public void Update(long lastModifiedUserId,
                            string accountTypeName,
                            TypeOfAccount type,
                            string description)
        {
            this.LastModifierUserId = lastModifiedUserId;
            this.LastModificationTime = Clock.Now;
            this.Type = type;
            this.AccountTypeName = accountTypeName;
            this.Description = description;
        }


        public void Enable(bool isEnable)
        {
            this.IsActive = isEnable;
        }

    }
}
