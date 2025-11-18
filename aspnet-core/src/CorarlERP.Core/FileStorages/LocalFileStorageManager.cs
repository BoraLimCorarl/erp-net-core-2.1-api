using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Abp.UI;
using OfficeOpenXml;

namespace CorarlERP.FileStorages
{
    public class LocalFileStorageManager : IFileStorageManager
    {
        protected readonly AppFolders _appFolders;

        public LocalFileStorageManager(AppFolders appFolders)
        {
            _appFolders = appFolders;
        }

        public async Task<string> GetTemplate(string templateName)
        {   
            var tokenPath = Path.Combine(_appFolders.TemplateFolder, templateName);
            if (!File.Exists(tokenPath))
            {
                throw new UserFriendlyException("FileNotFound");
            }

            var templateHtml = string.Empty;
            using (StreamReader r = new StreamReader(tokenPath))
            {
                return await r.ReadToEndAsync();
            }  
        }

        public async Task<Stream> DownloadTempFile(string fileToken)
        {
            var filePath = Path.Combine(_appFolders.TempFileDownloadFolder, fileToken);
            if (!System.IO.File.Exists(filePath))
            {
                throw new UserFriendlyException("RequestedFileDoesNotExists");
            }
            var memory = new MemoryStream();

            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            System.IO.File.Delete(filePath);

            return memory;
        }

        public async Task UploadTempFile(string fileToken, byte[] outBuffer)
        {
            var filePath = Path.Combine(_appFolders.TempFileDownloadFolder, fileToken);
            await File.WriteAllBytesAsync(filePath, outBuffer);
        }

        public async Task UploadTempFile(string fileToken, ExcelPackage excelPackage)
        {
            await Task.Run(() =>
            {
                var filePath = Path.Combine(_appFolders.TempFileDownloadFolder, fileToken);

                excelPackage.SaveAs(new FileInfo(filePath));
            });           
        }
    }
}
