using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.UI;
using CorarlERP.Configuration;
using Microsoft.Extensions.Configuration;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using OfficeOpenXml;

namespace CorarlERP.FileStorages
{
    public class AWSFileStorageManager : IFileStorageManager
    {
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IAmazonS3 _s3Client;
        public AWSFileStorageManager(
            IAppConfigurationAccessor configurationAccessor)
        {
            _appConfiguration = configurationAccessor.Configuration;
            _s3Client = new AmazonS3Client(AWSAccessKey, AWSSecretKey, Amazon.RegionEndpoint.GetBySystemName(AWSRegion));
        }


        public string AWSAccessKey => _appConfiguration["AWS:S3:AccessKey"];
        public string AWSSecretKey => _appConfiguration["AWS:S3:SecretKey"];
        public string AWSRegion => _appConfiguration["AWS:S3:Region"];
        public string AWSSessionToken => _appConfiguration["AWS:SessionToken"];
        public string AWSBucketNameTemplate => _appConfiguration["AWS:S3:BucketNameTemplate"];
        public string AWSBucketNameExport => _appConfiguration["AWS:S3:BucketNameExport"];

        public async Task<string> GetTemplate(string templateName)
        {

            var bucketName = AWSBucketNameTemplate;
            await CheckS3Bucket(bucketName);

            var s3Object = await _s3Client.GetObjectAsync(bucketName, templateName);

            using (StreamReader r = new StreamReader(s3Object.ResponseStream))
            {
                return await r.ReadToEndAsync();
            }
        
        }

        private async Task CheckS3Bucket(string bucketName)
        {
            var bucketExists = await _s3Client.DoesS3BucketExistAsync(bucketName);
            if (!bucketExists) throw new UserFriendlyException("MainFolderNotExists");
        }


        public async Task<Stream> DownloadTempFile(string fileToken)
        {
            var bucketName = AWSBucketNameExport;
            await CheckS3Bucket(bucketName);

            var s3Object = await _s3Client.GetObjectAsync(bucketName, fileToken);
            var result = s3Object.ResponseStream;
            await _s3Client.DeleteObjectAsync(bucketName, fileToken);
            return result;
        }

        public async Task UploadTempFile(string fileToken, byte[] outBuffer)
        {
            var bucketName = AWSBucketNameExport;

            await CheckS3Bucket(bucketName);

            var memory = new MemoryStream(outBuffer);         
            memory.Position = 0;

            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = fileToken,
                InputStream = memory
            };
            //request.Metadata.Add("Content-Type");
            await _s3Client.PutObjectAsync(request);
        }

        public async Task UploadTempFile(string fileToken, ExcelPackage excelPackage)
        {
            var bucketName = AWSBucketNameExport;

            await CheckS3Bucket(bucketName);

            var memory = new MemoryStream(excelPackage.GetAsByteArray());
            memory.Position = 0;

            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = fileToken,
                InputStream = memory
            };
            //request.Metadata.Add("Content-Type");
            await _s3Client.PutObjectAsync(request);

        }


    }
}
