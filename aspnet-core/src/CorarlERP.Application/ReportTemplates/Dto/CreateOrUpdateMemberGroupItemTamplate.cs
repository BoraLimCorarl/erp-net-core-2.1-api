using CorarlERP.UserGroups.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ReportTemplates.Dto
{
    public class CreateOrUpdateMemberGroupItemTamplate
    {
        public Guid? Id { get; set; }

        public long? MemberUserId { get; set; }
        public MemberDetail MemberUser { get; set; }

        public UserGroupDetailOutput UserGroup { get;  set; }
        public Guid? UserGroupId { get;  set; }
        
        public PermissionReadWrite PermissionReadWrite { get; set; }
    }
}
