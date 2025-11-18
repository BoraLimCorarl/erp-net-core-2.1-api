using CorarlERP.Addresses;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Customers.Dto
{
   public class CreateCustomerInput
    {
        [Required]
        public string CustomerCode { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Website { get; set; }
        public CAddress BillingAddress { get; set; }
        public CAddress ShippingAddress { get; set; }
        public string Remark { get; set; }
        public int? TenantId { get; set; }
        public bool SameAsShippngAddress { get; set; }
        public Guid? AccountId { get; set; }
        public long? CustomerTypeId { get; set; }
        public List<CreateOrUpdateCustomerContactPersonInput> ContactPersons { get; set; }
        public Member Member { get; set; }
        public List<GroupItems> UserGroups { get; set; }
        public bool IsWalkIn { get; set; }

        public long? LocationId { get; set; }
        public bool IsPOS { get; set; }
    }
}
