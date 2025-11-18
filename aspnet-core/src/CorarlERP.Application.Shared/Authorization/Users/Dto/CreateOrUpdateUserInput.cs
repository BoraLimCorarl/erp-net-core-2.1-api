using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CorarlERP.Authorization.Users.Dto
{
    public class CreateOrUpdateUserInput
    {
        [Required]
        public UserEditDto User { get; set; }

        [Required]
        public string[] AssignedRoleNames { get; set; }

        public bool SendActivationEmail { get; set; }

        public bool UseEmail { get; set; }

        public bool SetRandomPassword { get; set; }

        public List<long> OrganizationUnits { get; set; }

        public List<UserGroupItems> UserGroups { get; set; }

        public bool IsSpecificVendorType { get; set; }
        public List<VendorTypeMemberDto> VendorTypeMembers { get; set; }

        public bool IsSpecificCustomerType { get; set; }
        public List<CustomerTypeMemberDto> CustomerTypeMembers { get; set; }

        public CreateOrUpdateUserInput()
        {
            OrganizationUnits = new List<long>();
        }
    }

    public class UserGroupItems
    {
        public Guid? Id { get; set; }
        public long? UserId { get; set; }
        public long? LocationId { get; set; }
        public Guid GroupId { get; set; }
        public bool IsDefault { get; set; }
        public string GroupName { get; set; }
    }

    public class VendorTypeMemberDto
    {
        public long Id { get; set; }

        public long MemberId { get; set; }
        public string VendorTypeName { get; set; }
        public long VendorTypeId { get; set; }
    }

    public class CustomerTypeMemberDto
    {
        public long Id { get; set; }

        public long MemberId { get; set; }
        public string CustomerTypeName { get; set; }
        public long CustomerTypeId { get; set; }
    }

}