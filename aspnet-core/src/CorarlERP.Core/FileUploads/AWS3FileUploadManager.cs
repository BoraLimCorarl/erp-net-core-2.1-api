using Abp.Dependency;
using Abp.Domain.Uow;
using Amazon.S3;
using Amazon.S3.Model;
using CorarlERP.Galleries;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Abp.UI;
using CorarlERP.Configuration;
using Microsoft.Extensions.Configuration;

namespace CorarlERP.FileUploads
{
    public class AWS3FileUploadManager : BaseFileUploadManager, IAWS3FileUploadManager, ITransientDependency
    {
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IAmazonS3 _s3Client;
        public AWS3FileUploadManager(
            IAppConfigurationAccessor configurationAccessor)
        {
            _appConfiguration = configurationAccessor.Configuration;
            _s3Client = new AmazonS3Client(AWSAccessKey, AWSSecretKey, Amazon.RegionEndpoint.GetBySystemName(AWSRegion));
        }

        public string AWSAccessKey => _appConfiguration["AWS:S3:AccessKey"];
        public string AWSSecretKey => _appConfiguration["AWS:S3:SecretKey"];
        public string AWSRegion => _appConfiguration["AWS:S3:Region"];
        public string AWSSessionToken => _appConfiguration["AWS:SessionToken"];
        public string AWSBucketName => _appConfiguration["AWS:S3:BucketNameUpload"];

        [UnitOfWork(IsDisabled = true)]
        public async Task<Gallery> Upload(int? tenatId, long? curentUserId, UploadFrom uploadFrom, IFormFile file, string fileType)
        {
            var path = BuildPath(tenatId, file);
            var bucketName = this.AWSBucketName;

            await CheckS3Bucket(bucketName);

            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = path.FilePath,
                InputStream = file.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", file.ContentType);
            await _s3Client.PutObjectAsync(request);
                       

            var gallery = Gallery.Create(
                tenantId: tenatId,
                creatorUserId: curentUserId,
                name: path.DisplayName,
                contentType: fileType,
                storageFilePath: path.FilePath,
                storageMainFolderName: bucketName,
                fileSize: file.Length,
                uploadFrom: uploadFrom,
                uploadSource: UploadSource.AWS);

            return gallery;

        }

        [UnitOfWork(IsDisabled = true)]
        public async Task<Gallery> Upload(int? tenatId, long? curentUserId, UploadFrom uploadFrom, byte[] fileBytes, string fileType, string fileName)
        {
            var path = BuildPath(tenatId, fileName);
            var bucketName = this.AWSBucketName;

            await CheckS3Bucket(bucketName);

            var memory = new MemoryStream(fileBytes);
            memory.Position = 0;

            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = path.FilePath,
                InputStream = memory
            };
            request.Metadata.Add("Content-Type", fileType);
            await _s3Client.PutObjectAsync(request);


            var gallery = Gallery.Create(
                tenantId: tenatId,
                creatorUserId: curentUserId,
                name: path.DisplayName,
                contentType: fileType,
                storageFilePath: path.FilePath,
                storageMainFolderName: bucketName,
                fileSize: fileBytes.Length,
                uploadFrom: uploadFrom,
                uploadSource: UploadSource.AWS);

            return gallery;

        }


        [UnitOfWork(IsDisabled = true)]
        public async Task<GalleryDownloadOutput> Download(string mainFolderName, string storageFilePath, string contentType)
        {
            await CheckS3Bucket(mainFolderName);

            var s3Object = await _s3Client.GetObjectAsync(mainFolderName, storageFilePath);

            return new GalleryDownloadOutput()
            {
                Stream = s3Object.ResponseStream,
                ContentType = contentType
            };
        }

        private async Task CheckS3Bucket(string bucketName)
        {
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
            if (!bucketExists) throw new UserFriendlyException("MainFolderNotExists"); 
        }

        [UnitOfWork(IsDisabled = true)]
        public async Task<bool> Delete(string mainFolderName, string storageFilePath)
        {           
            try
            {
                await CheckS3Bucket(AWSBucketName);

                await _s3Client.DeleteObjectAsync(mainFolderName, storageFilePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
