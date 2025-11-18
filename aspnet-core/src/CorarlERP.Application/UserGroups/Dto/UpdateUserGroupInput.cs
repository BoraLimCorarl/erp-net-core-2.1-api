using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.UserGroups.Dto
{
   public class UpdateUserGroupInput : CreateUserGroupInput
    {
        public Guid Id { get; set; }
    }
}
