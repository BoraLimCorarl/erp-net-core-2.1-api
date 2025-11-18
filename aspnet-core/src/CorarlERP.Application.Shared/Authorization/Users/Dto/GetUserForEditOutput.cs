using System;
using System.Collections.Generic;
using CorarlERP.Organizations.Dto;

namespace CorarlERP.Authorization.Users.Dto
{
    public class GetUserForEditOutput
    {
        public Guid? ProfilePictureId { get; set; }

        public UserEditDto User { get; set; }

        public UserRoleDto[] Roles { get; set; }

        public List<OrganizationUnitDto> AllOrganizationUnits { get; set; }
        public List<UserGroupItems> UserGroups { get; set; }

        public List<string> MemberedOrganizationUnits { get; set; }

        public bool IsSpecificVendorType { get; set; }
        public List<VendorTypeMemberDto> VendorTypeMembers { get; set; }  
        public bool IsSpecificCustomerType { get; set; }
        public List<CustomerTypeMemberDto> CustomerTypeMembers { get; set; }
    }
}