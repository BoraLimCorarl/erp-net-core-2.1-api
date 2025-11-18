using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts;
using CorarlERP.VendorTypes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Vendors
{
    [Table("CarlErpVendors")]
    public class Vendor : AuditedEntity<Guid>, IMayHaveTenant
    {
        public const int MaxVendorCodeLength = 256;
        public const int MaxVendorNameLength = 512;
        public const int MaxEmailLength = 512;
        public const int MaxPhoneNumberLength = 128;
        public const int MaxWebsiteLength = 512;

        [Required]
        [MaxLength(MaxVendorCodeLength)]
        public string VendorCode { get; private set; }

        [Required]
        [MaxLength(MaxVendorNameLength)]
        public string VendorName { get; private set; }

        [DataType(DataType.EmailAddress)]
        [MaxLength(MaxEmailLength)]
        public string Email { get; private set; }

        [MaxLength(MaxPhoneNumberLength)]
        public string PhoneNumber { get; private set;}

        [MaxLength(MaxWebsiteLength)]
        public string Websit { get; private set; }
        public CAddress BillingAddress { get; private set; }
        public CAddress ShippingAddress { get;private set; }
        public string Remark { get; private set; }
        public int? TenantId { get; set; }
        public bool SameAsShippngAddress { get; private set; }

        public Guid? AccountId { get; private set; }
        public ChartOfAccount ChartOfAccount { get; private set; }
        public void SetAccount(Guid? accountId) => AccountId = accountId;
        [Required]
        public bool IsActive { get; private set; }

        public VendorType VendorType { get; private set; }
        public long? VendorTypeId { get; private set; }

        public Member Member { get; set; }
        public static Vendor Create(
            int? tenantId, long creatorUserId, string vendorCode, string vendorName,
            string email, string websit, string remark, CAddress billingAddress,
            CAddress shippingAddress, bool sameAsShippingAddress, string phoneNumber, 
            Guid? accountId, long? vendorTypeId)
        {
            return new Vendor()
            {
                VendorTypeId = vendorTypeId,
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                VendorCode = vendorCode,
                VendorName = vendorName,
                Email = email,
                Websit = websit,
                Remark = remark,
                IsActive = true,
                AccountId= accountId,
                BillingAddress = new CAddress(billingAddress.CityTown, billingAddress.Country, billingAddress.PostalCode, billingAddress.Province, billingAddress.Street),
                SameAsShippngAddress = sameAsShippingAddress,
                ShippingAddress = sameAsShippingAddress ? new CAddress(billingAddress.CityTown, billingAddress.Country, billingAddress.PostalCode, billingAddress.Province, billingAddress.Street) : 
                                                          new CAddress(shippingAddress.CityTown, shippingAddress.Country, shippingAddress.PostalCode, shippingAddress.Province, shippingAddress.Street),               
                PhoneNumber = phoneNumber
            };
        }
        public void Update(long lastModifiedUserId, string vendorCode, string vendorName,string email, string websit, string remark,
            CAddress billingAddress, CAddress shippingAddress, bool sameAsShippingAddress,string phoneNumber,Guid? accountId, long? vendorTypeId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            VendorCode = vendorCode;
            VendorName = vendorName;
            Email = email;
            Websit = websit;
            Remark = remark;
            AccountId = accountId;
            SameAsShippngAddress = sameAsShippingAddress;

            BillingAddress.Update(billingAddress);          
            ShippingAddress.Update(sameAsShippingAddress ? billingAddress : shippingAddress);

            PhoneNumber = phoneNumber;
            VendorTypeId = vendorTypeId;
        }
        public void Enable(bool isEnable)
        {
            IsActive = isEnable;
        }
        public void UpdateMember(Member member)
        {
            Member = member;
        }
    }
}
