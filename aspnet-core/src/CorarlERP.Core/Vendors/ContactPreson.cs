using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Vendors
{
    [Table("CarlErpContactPersons")]
    public class ContactPreson : AuditedEntity<Guid>, IMayHaveTenant
    {
        public const int MaxTitleLength = 64;
        public const int MaxNameLength = 512;
        public const int MaxFirstNameLength = 256;
        public const int MaxLastNameLength = 256;
        public const int MaxEmailLength = 512;
        public const int MaxPhoneNumberLength = 128;

        public int? TenantId { get; set; }

        [MaxLength(MaxTitleLength)]
        public string Title { get; private set; }
        [MaxLength(MaxFirstNameLength)]
        public string FirstName { get; private set; }
        [MaxLength(MaxLastNameLength)]
        public string LastName { get; private set; }
        [MaxLength(MaxNameLength)]
        public string DisplayNameAs { get; private set; }
        [MaxLength(MaxPhoneNumberLength)]
        public string PhoneNumber { get; private set; }
        public Guid VenderId { get; private set; }
        public Vendor Vendor { get; private set; } 
        public bool IsPrimary { get;private set; }

        [DataType(DataType.EmailAddress)]
        [MaxLength(MaxEmailLength)]
        public string Email { get; private set; }

        private static ContactPreson CreateContactPerson(int? tenantId, long? creatorUserId, string title, string firstName, string lastName, string displayNameAs, bool isPrimary, string phoneNumber,string email)
        {
            return new ContactPreson
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                TenantId = tenantId,
                Title = title,
                FirstName = firstName,
                LastName = lastName,
                DisplayNameAs = displayNameAs,
                IsPrimary = isPrimary,
                PhoneNumber = phoneNumber,
                Email = email
            };
        }

        //use for create contact person in Update vendor (when we have VendorId)
        public static ContactPreson CreateContactPerson(int? tenantId, long? creatorUserId,string title, string firstName,string lastName ,string displayNameAs,Guid vendorId, bool isPrimary,string phoneNumber,string email)
        {
            var result = CreateContactPerson(tenantId, creatorUserId, title, firstName, lastName, displayNameAs, isPrimary, phoneNumber,email);
            result.VenderId = vendorId;
            return result;
        }

        //user for create contact person in Create new vendor (when this vendor does not have Id yet)
        public static ContactPreson CreateContactPerson(int? tenantId, long? creatorUserId, string title, string firstName, string lastName, string displayNameAs, Vendor vendor, bool isPrimary, string phoneNumber,string email)
        {
            var result = CreateContactPerson(tenantId, creatorUserId, title, firstName, lastName, displayNameAs, isPrimary, phoneNumber,email);
            result.Vendor = vendor;
            return result;
        }


        //contact person cannot just change vendor 
        public void UpdateContactPerson(long lastModifiedUserId, string title, string firstName, string lastName, string displayNameAs,bool isPrimary,string phoneNumber,string email)
        {
            LastModifierUserId = lastModifiedUserId;
            Title = title;
            FirstName = firstName;
            LastName = lastName;
            DisplayNameAs = displayNameAs;
            IsPrimary = isPrimary;
            PhoneNumber = phoneNumber;
            Email = email;
        }

    }
}
