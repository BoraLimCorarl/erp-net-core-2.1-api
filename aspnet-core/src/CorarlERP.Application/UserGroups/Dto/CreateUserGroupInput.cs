using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.UserGroups.Dto
{
   public class CreateUserGroupInput
    {
        public string Name { get; set; }
        public List<CreateOrUpdateUserGroupMemberInput> UserGroupMember { get; set; }
    }
}
