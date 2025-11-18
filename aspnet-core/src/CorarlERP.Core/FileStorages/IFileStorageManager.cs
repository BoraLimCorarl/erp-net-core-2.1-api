using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OfficeOpenXml;

namespace CorarlERP.FileStorages
{
    public interface IFileStorageManager
    {
        Task<string> GetTemplate(string templateName);
        Task<Stream> DownloadTempFile(string fileToken);
        Task UploadTempFile(string fileToken, byte[] outBuffer);
        Task UploadTempFile(string fileToken, ExcelPackage excelPackage);
    }
}
