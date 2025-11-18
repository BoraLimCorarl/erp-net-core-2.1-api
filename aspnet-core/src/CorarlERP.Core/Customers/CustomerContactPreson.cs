using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Customers
{
    [Table("CarlErpCustomerContactPersons")]
    public class CustomerContactPerson : AuditedEntity<Guid>, IMayHaveTenant
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
        public Guid CustomerId { get; private set; }
        public Customer Customer { get; private set; } 
        public bool IsPrimary { get;private set; }

        [DataType(DataType.EmailAddress)]
        [MaxLength(MaxEmailLength)]
        public string Email { get; private set; }

        private static CustomerContactPerson CreateContactPerson(int? tenantId, long? creatorUserId, string title, string firstName, string lastName, string displayNameAs, bool isPrimary, string phoneNumber,string email)
        {
            return new CustomerContactPerson
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

        //use for create contact person in Update Customer (when we have CustomerId)
        public static CustomerContactPerson CreateContactPerson(int? tenantId, long? creatorUserId,string title, string firstName,string lastName ,string displayNameAs,Guid customerId, bool isPrimary,string phoneNumber,string email)
        {
            var result = CreateContactPerson(tenantId, creatorUserId, title, firstName, lastName, displayNameAs, isPrimary, phoneNumber,email);
            result.CustomerId = customerId;
            return result;
        }

        //user for create contact person in Create new Customer (when this Customer does not have Id yet)
        public static CustomerContactPerson CreateContactPerson(int? tenantId, long? creatorUserId, string title, string firstName, string lastName, string displayNameAs, Customer customer, bool isPrimary, string phoneNumber,string email)
        {
            var result = CreateContactPerson(tenantId, creatorUserId, title, firstName, lastName, displayNameAs, isPrimary, phoneNumber,email);
            result.Customer = customer;
            return result;
        }


        //contact person cannot just change Customer 
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
