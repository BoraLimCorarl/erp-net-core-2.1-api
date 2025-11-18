using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Locations
{
    [Table("CarlErpLocations")]
    public class Location : AuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxLocationNameLength = 512;
        public const int MaxPhoneNumberLength = 512;
        public int? TenantId { get; set; }

        [Required]
        [MaxLength(MaxLocationNameLength)]
        public string LocationName { get; private set; }

        public bool LocationParent { get; private set; }
        public Member Member { get; private set; }

        public long? ParentLocationId { get; private set; }
        public Location ParentLocation { get; private set; }

        [MaxLength(MaxPhoneNumberLength)]
        public string PhoneNumber { get; private set; }

        [Required]
        public bool IsActive { get; private set; }

        public static Location Create(int? tenantId, long creatorUserId, string locationName, bool locationParent, Member member, long? parentLocationId,string phoneNumber)
        {
            return new Location()
            {
                Member = member,
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                IsActive = true,
                LocationParent = locationParent,
                LocationName = locationName,
                ParentLocationId = parentLocationId,
                PhoneNumber = phoneNumber,

            };
        }
        public void Enable(bool isEnable)
        {
            IsActive = isEnable;
        }
        public void Update(long lastModifiedUserId, string locationName, bool locationParent, Member member, long? parentlocationId, string phoneNumber)
        {
            Member = member;
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            LocationName = locationName;
            LocationParent = locationParent;
            ParentLocationId = parentlocationId;
            PhoneNumber = phoneNumber;
        }
    }
}
