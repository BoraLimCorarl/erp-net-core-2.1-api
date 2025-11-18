using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Referrals
{
    [Table("CarlErpReferrals")]
    public class Referral : AuditedEntity<long>
    {
        public const int MaxNameLength = 32;
        public const int MaxCodelength = 32;     
        [Required]
        [MaxLength(MaxCodelength)]
        public string Code { get; private set; }
        [Required]
        [MaxLength(MaxNameLength)]
        public string Name { get; private set; }
        public DateTime? ExpirationDate { get; private set; }
        public string Description { get; private set; }
        public int? Qty { get; private set; }
        public bool IsActive { get; private set; }
        public static Referral Create(
                                long? creatorUserId,
                                string name,
                                int? qty,
                                DateTime? expirationDate,
                                string code,
                                string description)
        {
            return new Referral()
            {
              
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                Name = name,
                Qty = qty,
                IsActive = true,
                ExpirationDate = expirationDate,
                Code = code,
                Description = description
            };
        }

        public void Update(long lastModifiedUserId, string name, int? qty, DateTime? expirationDate,string code,string description)
        {
            this.LastModifierUserId = lastModifiedUserId;
            this.LastModificationTime = Clock.Now;
            this.Name = name;
            this.Qty = qty;
            this.Description = description;
            this.ExpirationDate = expirationDate;
            this.Code = code;              
        } 
        public void UpdateStatus(bool status)
        {
            this.IsActive = status;
        }

    }
}
