using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Authorization.Users;
using CorarlERP.UserGroups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ReportTemplates
{
    [Table("CarlErpGroupMemberItemTamplates")]
    public class GroupMemberItemTamplate : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get ; set; }

        public ReportTemplate ReportTemplate { get; set; }
        public long ReportTemplateId { get; private set; }

        public UserGroup UserGroup { get; private set; }
        public Guid? UserGroupId { get; private set; }
        
        public User MemberUser { get; private set; }
        public long? MemberUserId { get; private set; }

        public PermissionReadWrite PermissionReadWrite { get; private set; }

        private static GroupMemberItemTamplate Create
            (int? tenantId, long? creatorUserId, 
            Guid? userGroupId,long? memberUserId,
             PermissionReadWrite permissionReadWrite)
        {
            return new GroupMemberItemTamplate
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                TenantId = tenantId,
                PermissionReadWrite = permissionReadWrite,
                MemberUserId = memberUserId,
                UserGroupId = userGroupId,
                
            };
        }

        public static GroupMemberItemTamplate Create(int? tenantId, 
                            long? creatorUserId,
                            Guid? userGroupId, long? memberUserId,
                            PermissionReadWrite permissionReadWrite,
                            long reportTemplateId)
        {
            var result = Create(tenantId,creatorUserId,userGroupId,memberUserId,permissionReadWrite);
            result.ReportTemplateId = reportTemplateId;
            return result;
        }

        public static GroupMemberItemTamplate Create(int? tenantId,
                            long? creatorUserId,
                            Guid? userGroupId, long? memberUserId,
                            PermissionReadWrite permissionReadWrite,
                            ReportTemplate reportTemplate)
        {
            var result = Create(tenantId, creatorUserId,userGroupId,memberUserId,permissionReadWrite);
            result.ReportTemplate = reportTemplate;
            return result;
        }

        public void Update(long lastModifiedUserId, Guid? userGroupId, long? memberUserId,
                            PermissionReadWrite permissionReadWrite )
        {
            LastModifierUserId = lastModifiedUserId;
            UserGroupId = userGroupId;
            MemberUserId = memberUserId;
            PermissionReadWrite = permissionReadWrite;


        }

    }
}
