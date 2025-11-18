using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Uow;
using System.Transactions;
using System.Threading.Tasks;
using Abp.UI;
using Microsoft.AspNetCore.Http;
using ByteSizeLib;
using Abp.Extensions;
using Abp.Dependency;
using CorarlERP.Galleries;
using Microsoft.Extensions.Configuration;
using CorarlERP.Configuration;
using CorarlERP.ReportTemplates;
using Abp.Domain.Repositories;

namespace CorarlERP.FileUploads
{
    public class FileUploadManager : IFileUploadManager, ITransientDependency
    {
        private readonly IGalleryManager _galleryManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ILocalFileUploadManager _localFileUploadManager;
        private readonly IAWS3FileUploadManager _aws3FileUploadManager;
        private readonly IConfigurationRoot _appConfiguration;

        public FileUploadManager(
            IAppConfigurationAccessor configurationAccessor,
            IUnitOfWorkManager unitOfWorkManager,
            ILocalFileUploadManager localFileUploadManager,
            IAWS3FileUploadManager aws3FileUploadManager,
            IGalleryManager galleryManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _localFileUploadManager = localFileUploadManager;
            _aws3FileUploadManager = aws3FileUploadManager;
            _galleryManager = galleryManager;
            _appConfiguration = configurationAccessor.Configuration;
        }

        public bool AwsS3Enable => _appConfiguration["AWS:S3:Enable"].ToLower() == "true";

        [UnitOfWork(IsDisabled = true)]
        public async Task<UploadGalleryFileOutput> Upload(int? tenantId, long? curentUserId, UploadFrom uploadFrom, IFormFile file, string fileType)
        {

            Gallery gallery = null;

            if (AwsS3Enable)
            {
                gallery = await _aws3FileUploadManager.Upload(tenantId, curentUserId, uploadFrom, file, fileType);
            }
            else 
            {
                gallery = await _localFileUploadManager.Upload(tenantId, curentUserId, uploadFrom, file, fileType);
            }

            try
            {
                await SaveGalleryHelper(tenantId, gallery);
            }
            catch (Exception ex)
            {
                if (AwsS3Enable)
                {
                    await _aws3FileUploadManager.Delete(gallery.StorageMainFolderName, gallery.StorageFilePath);
                }
                else 
                {
                    await _localFileUploadManager.Delete(gallery.StorageMainFolderName, gallery.StorageFilePath);
                }

                throw ex;
            }

            return new UploadGalleryFileOutput()
            {
                FileName = gallery.Name,
                FileSize = GetFileSizeInBytes(gallery.FileSize),
                FileType = GetFileContentType(fileType),
                GalleryId = gallery.Id
            };
        }


        [UnitOfWork(IsDisabled = true)]
        public async Task<UploadGalleryFileOutput> Upload(int? tenantId, long? curentUserId, UploadFrom uploadFrom, byte[] fileBytes, string fileType, string fileName)
        {

            Gallery gallery = null;

            if (AwsS3Enable)
            {
                gallery = await _aws3FileUploadManager.Upload(tenantId, curentUserId, uploadFrom, fileBytes, fileType, fileName);
            }
            else
            {
                gallery = await _localFileUploadManager.Upload(tenantId, curentUserId, uploadFrom, fileBytes, fileType, fileName);
            }

            try
            {
                await SaveGalleryHelper(tenantId, gallery);
            }
            catch (Exception ex)
            {
                if (AwsS3Enable)
                {
                    await _aws3FileUploadManager.Delete(gallery.StorageMainFolderName, gallery.StorageFilePath);
                }
                else
                {
                    await _localFileUploadManager.Delete(gallery.StorageMainFolderName, gallery.StorageFilePath);
                }

                throw ex;
            }

            return new UploadGalleryFileOutput()
            {
                FileName = gallery.Name,
                FileSize = GetFileSizeInBytes(gallery.FileSize),
                FileType = GetFileContentType(fileType),
                GalleryId = gallery.Id
            };
        }


        private string GetFileContentType(string fileType)
        {
            if (fileType.IsNullOrEmpty()) return "";
            string[] words = fileType.Split('/');
            if (words.Length > 1 && words[1] != null)
            {
                return words[1].ToUpper();
            }
            else if (words.Length == 1)
            {
                return words[0].ToUpper();
            }
            else
            {
                return "";
            }
        }

        private string GetFileSizeInBytes(long TotalBytes)
        {
            return ByteSize.FromBytes(TotalBytes).ToString();
            // 1 MB //if (TotalBytes >= 1073741824) //Giga Bytes
            //{
            // Decimal FileSize = Decimal.Divide(TotalBytes, 1073741824);
            // return String.Format("{0:##.#} GB", FileSize);
            //}
            //else if (TotalBytes >= 1048576) //Mega Bytes
            //{
            // Decimal FileSize = Decimal.Divide(TotalBytes, 1048576);
            // return String.Format("{0:##.#} MB", FileSize);
            //}
            //else if (TotalBytes >= 1024) //Kilo Bytes
            //{
            // Decimal FileSize = Decimal.Divide(TotalBytes, 1024);
            // return String.Format("{0:##.#} KB", FileSize);
            //}
            //else if (TotalBytes > 0)
            //{
            // Decimal FileSize = TotalBytes;
            // return String.Format("{0:##.#} Bytes", FileSize);
            //}
            //else
            //{
            // return "0 Bytes";
            //}

        }

        [UnitOfWork(IsDisabled = true)]
        private async Task<Gallery> SaveGalleryHelper(int? tenantId, Gallery gallery)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    await _galleryManager.CreateAsync(gallery);
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
                await uow.CompleteAsync();
            }

            return gallery;
        }


        [UnitOfWork(IsDisabled = true)]
        public async Task Delete(int? tenantId, Guid galleryId)
        {
            try
            {
                var gallery = await GetGalleryHelper(tenantId, galleryId);

                if (gallery.UploadSource == UploadSource.AWS)
                {
                    await _aws3FileUploadManager.Delete(gallery.StorageMainFolderName, gallery.StorageFilePath);
                }
                else if (gallery.UploadSource == UploadSource.Local)
                {
                    await _localFileUploadManager.Delete(gallery.StorageMainFolderName, gallery.StorageFilePath);
                }

                using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        await _galleryManager.DeleteAsync(gallery);
                    }
                    await uow.CompleteAsync();
                }
            }
            catch (UserFriendlyException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("CannotDelete");
            }
        }


        [UnitOfWork(IsDisabled = true)]
        public async Task<GalleryDownloadOutput> DownLoad(int? tenantId, Guid id)
        {
            var gallery = await GetGalleryHelper(tenantId, id);

            GalleryDownloadOutput galleryFile = null;

            if (gallery.UploadSource == UploadSource.AWS)
            {
                galleryFile = await _aws3FileUploadManager.Download(gallery.StorageMainFolderName, gallery.StorageFilePath, gallery.ContentType);
            }
            else if (gallery.UploadSource == UploadSource.Local)
            {
                galleryFile = await _localFileUploadManager.Download(gallery.StorageMainFolderName, gallery.StorageFilePath, gallery.ContentType);
            }
            else
            {
                throw new UserFriendlyException("FileNotFound");
            }

            galleryFile.FileName = gallery.Name;

            return galleryFile;
        }

        [UnitOfWork(IsDisabled = true)]
        private async Task<Gallery> GetGalleryHelper(int? tenantId, Guid id, bool tracking = false)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    var gallery = await _galleryManager.GetAsync(id, tracking);

                    if (gallery == null) throw new UserFriendlyException("RecordNotFound");

                    return gallery;
                }
            }
        }

    }
}
