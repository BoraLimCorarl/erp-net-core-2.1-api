using Abp.Dependency;
using Abp.Domain.Services;
using CorarlERP.Galleries;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.FileUploads
{
    public interface ILocalFileUploadManager 
    {
        Task<Gallery> Upload(int? tenatId, long? curentUserId, UploadFrom uploadFrom, IFormFile file, string fileType);
        Task<Gallery> Upload(int? tenantId, long? curentUserId, UploadFrom uploadFrom, byte[] fileBytes, string fileType, string fileName);
        Task<GalleryDownloadOutput> Download(string mainFolderName, string storageFilePath, string contentType);
        Task<bool> Delete(string mainFolderName, string storageFilePath);
    }
}
