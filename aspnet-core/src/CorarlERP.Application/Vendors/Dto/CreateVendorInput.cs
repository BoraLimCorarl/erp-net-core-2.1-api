using CorarlERP.Addresses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Vendors.Dto
{
    public class CreateVendorInput
    {
        [Required]
        public string VendorCode { get; set; }
        [Required]
        public string VendorName { get; set; }
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
        public List<CreateOrUpdateContactPersonInput> ContactPersons { get; set; }

        public Member Member { get; set; }
        public List<GroupItems> UserGroups { get; set; }
        public long? VendorTypeId { get; set; }
    }

    public class GroupItems
    {
        public Guid? Id { get; set; }
        public Guid UserGroupId { get; set; }
        public string UserGroupName { get; set; }

    }
}