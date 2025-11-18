using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http.Headers;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace CorarlERP.FileUploads
{
    public abstract class BaseFileUploadManager
    {
        protected FilePathBuildOutputObj BuildPath(int? tenantId, IFormFile file)
        {
            var tenantPath = tenantId.HasValue ? $"Tenant_{tenantId}" : "Host";

            var dateTime = DateTime.UtcNow;
            var subFolderName = $"{dateTime.ToString("yyyy")}_{dateTime.ToString("MM")}";
            
            //Documents/Tenant_1/2022_06
            var folderName = Path.Combine(CorarlERPConsts.DocumentFolder, tenantPath, subFolderName);
            var displayName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var storageName = $"{Guid.NewGuid().ToString("N").ToUpperInvariant()}.{displayName.Split('.').LastOrDefault()}";
            
            //Documents/Tenant_1/2022_06/0000-0000-0000-0000-0000.jpg
            var fullPath = Path.Combine(folderName, storageName);

            return new FilePathBuildOutputObj
            {
                FilePath = fullPath,
                StarageName = storageName,
                DisplayName = displayName,
                FolderName = folderName
            };
        }

        protected FilePathBuildOutputObj BuildPath(int? tenantId, string displayName)
        {
            var tenantPath = tenantId.HasValue ? $"Tenant_{tenantId}" : "Host";

            var dateTime = DateTime.UtcNow;
            var subFolderName = $"{dateTime.ToString("yyyy")}_{dateTime.ToString("MM")}";

            //Documents/Tenant_1/2022_06
            var folderName = Path.Combine(CorarlERPConsts.DocumentFolder, tenantPath, subFolderName);           
            var storageName = $"{Guid.NewGuid().ToString("N").ToUpperInvariant()}.{displayName.Split('.').LastOrDefault()}";

            //Documents/Tenant_1/2022_06/0000-0000-0000-0000-0000.jpg
            var fullPath = Path.Combine(folderName, storageName);

            return new FilePathBuildOutputObj
            {
                FilePath = fullPath,
                StarageName = storageName,
                DisplayName = displayName,
                FolderName = folderName
            };
        }
    }
}
