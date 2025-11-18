using Abp.Dependency;
using Abp.Domain.Uow;
using CorarlERP.Galleries;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using Abp.UI;

namespace CorarlERP.FileUploads
{
    public class LocalFileUploadManager : BaseFileUploadManager, ILocalFileUploadManager, ITransientDependency
    {
        private string MainFolder = "zobookk-resources";
        public LocalFileUploadManager()
        {
        }

        [UnitOfWork(IsDisabled = true)]
        public async Task<Gallery> Upload(int? tenatId, long? curentUserId, UploadFrom uploadFrom, IFormFile file, string fileType)
        {
            //var dateTime = DateTime.UtcNow;
            //var subFolderName = $"{dateTime.ToString("yyyy")}_{dateTime.ToString("MM")}";
            //var folderName = Path.Combine(CorarlERPConsts.DocumentFolder, CorarlERPConsts.BuildTenantFolderName(tenatId), subFolderName);
            //var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            //Directory.CreateDirectory(pathToSave);
            //var displayName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            //var storageName = $"{Guid.NewGuid().ToString("N").ToUpperInvariant()}.{displayName.Split('.').LastOrDefault()}";
            //var fullPath = Path.Combine(pathToSave, storageName);

            var path = BuildPath(tenatId, file);

            //C://Zobookk/zobookk-resources/Documents/Tenant_1/2022_06
            var folderToSave = Path.Combine(Directory.GetCurrentDirectory(), MainFolder, path.FolderName);
            Directory.CreateDirectory(folderToSave);

            //C://Zobookk/zobookk-resources/Documents/Tenant_1/2022_06/0000-0000-0000-0000-0000.jpg
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), MainFolder, path.FilePath);            

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var gallery = Gallery.Create(
                tenantId: tenatId, 
                creatorUserId: curentUserId, 
                name: path.DisplayName, 
                contentType: fileType, 
                storageFilePath: path.FilePath, 
                storageMainFolderName: MainFolder, 
                fileSize: file.Length, 
                uploadFrom: uploadFrom, 
                uploadSource : UploadSource.Local);
         
            return gallery;
            
        }

        [UnitOfWork(IsDisabled = true)]
        public async Task<Gallery> Upload(int? tenatId, long? curentUserId, UploadFrom uploadFrom, byte[] fileBytes, string fileType, string fileName)
        {
            //var dateTime = DateTime.UtcNow;
            //var subFolderName = $"{dateTime.ToString("yyyy")}_{dateTime.ToString("MM")}";
            //var folderName = Path.Combine(CorarlERPConsts.DocumentFolder, CorarlERPConsts.BuildTenantFolderName(tenatId), subFolderName);
            //var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            //Directory.CreateDirectory(pathToSave);
            //var displayName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            //var storageName = $"{Guid.NewGuid().ToString("N").ToUpperInvariant()}.{displayName.Split('.').LastOrDefault()}";
            //var fullPath = Path.Combine(pathToSave, storageName);

            var path = BuildPath(tenatId, fileName);

            //C://Zobookk/zobookk-resources/Documents/Tenant_1/2022_06
            var folderToSave = Path.Combine(Directory.GetCurrentDirectory(), MainFolder, path.FolderName);
            Directory.CreateDirectory(folderToSave);

            //C://Zobookk/zobookk-resources/Documents/Tenant_1/2022_06/0000-0000-0000-0000-0000.jpg
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), MainFolder, path.FilePath);
            await File.WriteAllBytesAsync(fullPath, fileBytes);

            var gallery = Gallery.Create(
                tenantId: tenatId,
                creatorUserId: curentUserId,
                name: path.DisplayName,
                contentType: fileType,
                storageFilePath: path.FilePath,
                storageMainFolderName: MainFolder,
                fileSize: fileBytes.Length,
                uploadFrom: uploadFrom,
                uploadSource: UploadSource.Local);

            return gallery;

        }


        [UnitOfWork(IsDisabled = true)]
        public async Task<GalleryDownloadOutput> Download(string mainFolderName, string storageFilePath, string contentType)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), mainFolderName, storageFilePath);
            
            var memory = new MemoryStream();

            using (var stream = new FileStream(fullPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0; 
            
            return new GalleryDownloadOutput()
            {
                Stream = memory,
                ContentType = contentType
            };
        }

        [UnitOfWork(IsDisabled = true)]
        public async Task<bool> Delete(string mainFolderName, string storageFilePath)
        {
            return await Task.Run(() => {
                try
                {
                    var path = Path.Combine(mainFolderName, storageFilePath);
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    return true;
                }
                catch
                {
                    return false;
                }

            });            
        }

    }
}
