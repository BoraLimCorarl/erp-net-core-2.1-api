using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Galleries
{

    [Table("CarlErpGalleries")]
    public class Gallery : AuditedEntity<Guid>, IAudited, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [Required]
        [StringLength(CorarlERPConsts.MaxNameLength)]
        public string Name { get; private set; } //Display Name
        public long FileSize { get; private set; }
        [Required]
        [StringLength(CorarlERPConsts.MaxNameLength)]
        public string ContentType { get; private set; }
        [Required]
        [StringLength(CorarlERPConsts.MaxFilePathLength)]
        public string StorageFilePath { get; private set; }

        public UploadFrom UploadFrom { get; private set; }
        public UploadSource UploadSource { get; private set; }

        [Required]
        [StringLength(CorarlERPConsts.MaxNameLength)]
        public string StorageMainFolderName { get; private set; }

        public static Gallery Create(int? tenantId, long? creatorUserId,
        string name,
        string contentType,
        string storageFilePath,
        string storageMainFolderName,
        long fileSize,
        UploadFrom uploadFrom,
        UploadSource uploadSource
        )
        {
            var result = new Gallery()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreationTime = Clock.Now,
                CreatorUserId = creatorUserId,
                Name = name,
                ContentType = contentType,
                FileSize = fileSize,
                StorageFilePath = storageFilePath,
                StorageMainFolderName = storageMainFolderName,
                UploadFrom = uploadFrom,
                UploadSource = uploadSource
            };

            return result;
        }

        public void UpdateName(long? lastModifierUserId,
        string name)
        {
            this.LastModificationTime = Clock.Now;
            this.LastModifierUserId = lastModifierUserId;
            this.Name = name; //we will allow user to change name but wont change actual file storage name
        }
        public void updateName(string name)
        {
            Name = name;
        }
        public void UpdateTenantId(int? tenantId)
        {
            TenantId = tenantId;
        }
    }


}
