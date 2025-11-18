using Abp.Domain.Entities;
using Abp.Timing;
using CorarlERP.MultiTenancy;
using CorarlERP.Referrals;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.SignUps
{
    [Table("CarlErpSignUps")]
    public class SignUp : Entity<Guid>
    {
        public enum EnumStatus
        {
            Pending = 0,          
            FreeTrail = 1,
            Subscribed = 2,
            NoteLigible =3
        };
        public const int MaxLength = 128;

        [MaxLength(MaxLength)]
        [Required]
        public string FirstName { get; private set; }
        [MaxLength(MaxLength)]
        [Required]
        public string LastName { get; private set; }
        [Required]
        public string Email { get; private set; }
        [MaxLength(MaxLength)]
        [Required]
        public string CompanyOrStoreName { get; private set; }
        [MaxLength(MaxLength)]
        [Required]
        public string Position { get; private set; }
        [MaxLength(MaxLength)]
        [Required]
        public string PhoneNumber { get; private set; }

        [MaxLength(MaxLength)]
        public string SignUpCode { get; private set; }
        public bool IsActive { get; private set; }

        public DateTime CreationTime { get; private set; }

        public Tenant Tenant { get; private set; }
        public int? TenantId { get; private set; }

        public string Description { get; private set; }

        public Referral Referral { get; private set; }
        public long? ReferralId { get; private set; }

        public EnumStatus Status { get; private set; }

        public static SignUp Create(string firstName, string lastName, string email, string companyOrStorName, string posistion, string phoneNumber, string signUpCode)
        {
            return new SignUp()
            {
                CompanyOrStoreName = companyOrStorName,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Id = Guid.NewGuid(),
                PhoneNumber = phoneNumber,
                Position = posistion,
                SignUpCode = signUpCode,
                IsActive = true,
                CreationTime = Clock.Now,
            };
        }
        public void UpdateEnumStatus (EnumStatus status)
        {
            this.Status = status;
        }
        public void SetReferralAndDescription(string description, long? referralId)
        {
            this.Description = description;
            this.ReferralId = referralId;
        }
        public void UpdateStatus(bool isActive)
        {
            this.IsActive = isActive;
        }


      

        public void Update(string firstName, string lastName, string email, string companyOrStorName, string posistion, string phoneNumber, string signUpCode)
        {
            CompanyOrStoreName = companyOrStorName;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Position = posistion;
            SignUpCode = signUpCode;
        }
        public void UpdateTenant(int? tenantId)
        {
            this.TenantId = tenantId;
        }
    }
}
