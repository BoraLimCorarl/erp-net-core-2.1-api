using Abp.AutoMapper;
using CorarlERP.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.UserGroups.Dto
{
    [AutoMapFrom(typeof(UserGroupMember))]
   public class UserGroupMemberDetail
    {
        public Guid Id { get; set; }
        public long MemberId { get; set; }
        public MemberDetail Member { get; set; }
    }

    [AutoMapFrom(typeof(User))]
    public class MemberDetail
    {
        public string SurName { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
    }

}
