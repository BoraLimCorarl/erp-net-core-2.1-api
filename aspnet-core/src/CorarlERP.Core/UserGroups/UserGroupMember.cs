using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CorarlERP.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorarlERP.UserGroups
{
    [Table("CarlErpUserGroupMembers")]
    public class UserGroupMember : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get ; set; }

        public long MemberId { get; private set; }
        public User Member { get; private set; }
        public bool IsDefault { get; private set; }
        public Guid UserGroupId { get; private set; }
        public UserGroup UserGroup { get; private set; }

        private static UserGroupMember CreateUserGroupMember(int? tenantId, long? creatorUserId, long memberId)
        {
            return new UserGroupMember
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                TenantId = tenantId,
                MemberId = memberId
            };
        }

        public static UserGroupMember CreateUserGroupMember(int? tenantId, long? creatorUserId,long memberId, Guid userGroupId)
        {
            var result = CreateUserGroupMember(tenantId, creatorUserId,memberId);
            result.UserGroupId = userGroupId;
            return result;
        }

        public static UserGroupMember CreateUserGroupMember(int? tenantId, long? creatorUserId,long memberId, UserGroup userGroup )
        {
            var result = CreateUserGroupMember(tenantId, creatorUserId,memberId);
            result.UserGroup = userGroup;
            return result;
        }
        
        public void UpdateUserGroupMember(long lastModifiedUserId, long memberId)
        {
            LastModifierUserId = lastModifiedUserId;
            MemberId = memberId;
        }

        public void UpdateIsDefault(bool isDefault)
        {
            IsDefault = isDefault;
        }
    }
}
