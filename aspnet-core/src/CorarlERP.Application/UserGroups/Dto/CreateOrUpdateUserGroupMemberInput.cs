using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.UserGroups.Dto
{
   public class CreateOrUpdateUserGroupMemberInput
    {
        public Guid? Id { get; set; }
        public long MemberId { get; set; }
    }
}
