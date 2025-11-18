using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.UserGroups.Dto
{
    [AutoMapFrom(typeof(UserGroup))]
    public class UserGroupDetailOutput
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public long? LocationId { get; set; }
        public List<UserGroupMemberDetail> UserGroupMemberDetails { get; set; }
    }
}
