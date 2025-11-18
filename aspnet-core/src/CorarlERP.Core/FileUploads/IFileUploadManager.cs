using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using CorarlERP.Galleries;

namespace CorarlERP.FileUploads
{
    public interface IFileUploadManager
    {
        Task Delete(int? tenantId, Guid galleryId);
        Task<GalleryDownloadOutput> DownLoad(int? tenantId, Guid id);
        Task<UploadGalleryFileOutput> Upload(int? tenantId, long? curentUserId, UploadFrom uploadFrom, IFormFile file, string fileType);
        Task<UploadGalleryFileOutput> Upload(int? tenantId, long? curentUserId, UploadFrom uploadFrom, byte[] fileBytes, string fileType, string fileName);
    }
}
