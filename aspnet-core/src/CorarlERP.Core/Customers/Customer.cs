using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts;
using CorarlERP.CustomerTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Customers
{
    [Table("CarlErpCustomers")]
    public class Customer : AuditedEntity<Guid>, IMayHaveTenant
    {

        public const int MaxCustomerCodeLength = 256;
        public const int MaxCustomerNameLength = 512;
        public const int MaxEmailLength = 512;
        public const int MaxPhoneNumberLength = 128;
        public const int MaxWebsiteLength = 512;

        [Required]
        [MaxLength(MaxCustomerCodeLength)]
        public string CustomerCode { get; private set; }

        [Required]
        [MaxLength(MaxCustomerNameLength)]
        public string CustomerName { get; private set; }

        [DataType(DataType.EmailAddress)]
        [MaxLength(MaxEmailLength)]
        public string Email { get; private set; }

        [MaxLength(MaxPhoneNumberLength)]
        public string PhoneNumber { get; private set; }

        [MaxLength(MaxWebsiteLength)]
        public string Website { get; private set; }
        public CAddress BillingAddress { get; private set; }
        public CAddress ShippingAddress { get; private set; }
        public string Remark { get; private set; }
        public int? TenantId { get; set; }
        public bool SameAsShippngAddress { get; private set; }

        public Guid? AccountId { get; private set; }
        public ChartOfAccount Account { get; private set; }
        public void SetAccount(Guid? accountId) => AccountId = accountId;
        [Required]
        public bool IsActive { get; private set; }
     
        public CustomerType CustomerType { get; private set; }
        public long? CustomerTypeId { get; private set; }

        public Member Member { get; set; }
        public bool IsWalkIn { get; private set; }
        public static Customer Create(int? tenantId, long creatorUserId, string customerCode, string customerName,
        string email, string website, string remark, CAddress billingAddress, CAddress shippingAddress, bool sameAsShippingAddress, 
        string phoneNumber, Guid? accountId, long? customerTypeId,bool isWorkIn)
        {
            return new Customer()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                CustomerCode = customerCode,
                CustomerName = customerName,
                Email = email,
                Website = website,
                Remark = remark,
                IsActive = true,
                AccountId = accountId,
                BillingAddress = new CAddress(billingAddress.CityTown, billingAddress.Country, billingAddress.PostalCode, billingAddress.Province, billingAddress.Street),
                SameAsShippngAddress = sameAsShippingAddress,
                ShippingAddress = sameAsShippingAddress ? new CAddress(billingAddress.CityTown, billingAddress.Country, billingAddress.PostalCode, billingAddress.Province, billingAddress.Street) :
                                                          new CAddress(shippingAddress.CityTown, shippingAddress.Country, shippingAddress.PostalCode, shippingAddress.Province, shippingAddress.Street),
                PhoneNumber = phoneNumber,
                CustomerTypeId = customerTypeId,
                IsWalkIn = isWorkIn,

            };
        }

        public void Update(long lastModifiedUserId, string customerCode, string customerName, string email, string website, string remark,
         CAddress billingAddress, CAddress shippingAddress, bool sameAsShippingAddress, string phoneNumber, Guid? accountId,long? customerTypeId, bool isWorkIn)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            CustomerCode = customerCode;
            CustomerName = customerName;
            Email = email;
            Website = website;
            Remark = remark;
            AccountId = accountId;
            SameAsShippngAddress = sameAsShippingAddress;

            BillingAddress.Update(billingAddress);
            ShippingAddress.Update(sameAsShippingAddress ? billingAddress : shippingAddress);

            PhoneNumber = phoneNumber;
            CustomerTypeId = customerTypeId;
            IsWalkIn = isWorkIn;
        }

        public void UpdateMember(Member member)
        {
            Member = member;
        }
        public void Enable(bool isEnable)
        {
            IsActive = isEnable;
        }
    }
}
