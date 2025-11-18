using System;
using System.Net;
using System.Threading.Tasks;
using Abp.Auditing;
using Microsoft.AspNetCore.Mvc;
using CorarlERP.Dto;
using CorarlERP.Storage;
using Abp.UI;
using System.IO;
using Abp.Web.Models;
using CorarlERP.FileStorages;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.IO.Extensions;
using CorarlERP.Authorization.Users.Profile.Dto;
using CorarlERP.Web.Helpers;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;

namespace CorarlERP.Web.Controllers
{
    public class FileController : CorarlERPControllerBase
    {
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBinaryObjectManager _binaryObjectManager;

        private readonly IAppFolders _appFolders;
        private readonly IFileStorageManager _fileStorageManager;

        public FileController(
            ITempFileCacheManager tempFileCacheManager,
            IBinaryObjectManager binaryObjectManager,
            IAppFolders appFolders,
            IFileStorageManager fileStorageManager
        )
        {
            _tempFileCacheManager = tempFileCacheManager;
            _binaryObjectManager = binaryObjectManager;
            _appFolders = appFolders;
            _fileStorageManager = fileStorageManager;
        }

        [DisableAuditing]
        public async Task<ActionResult> DownloadTempFile(FileDto file)
        {

            #region new implement 
            try
            {

                var extensionArray = file.FileName.Contains(".") ? file.FileName.Split(".") : null;

                if (extensionArray == null)
                {
                    throw new UserFriendlyException(L("InvalidExtension"));
                }
                var extension = extensionArray[extensionArray.Length - 1].ToLower();

                var contentType = ConvertExtentionToContentType(extension);

                var fileBytes = await _fileStorageManager.DownloadTempFile(file.FileToken);
                return File(fileBytes, contentType, file.FileName);
            }
            catch (UserFriendlyException ex)
            {
                throw new UserFriendlyException(L(ex.Message));
               
            }

            

            #endregion

            #region original one
            //var fileBytes = _tempFileCacheManager.GetFile(file.FileToken);
            //if (fileBytes == null)
            //{
            //    return NotFound(L("RequestedFileDoesNotExists"));
            //}

            //return File(fileBytes, file.FileType, file.FileName);
            #endregion
        }


        [AbpMvcAuthorize]
        public async Task<JsonResult> UploadTempFile(FileDto input)
        {
            try
            {
                //Check input
                if (Request.Form.Files.Count <= 0 || Request.Form.Files[0] == null)
                {
                    throw new UserFriendlyException(L("ImportFileError"));
                }
                var file = Request.Form.Files[0];
                //Guid guid = Guid.NewGuid();
                var fileInfo = new FileInfo(file.FileName);

                if (file.Length > (5242880 * 10)) //1MB * 10.
                {
                    throw new UserFriendlyException(L("ImportFileExcelWarnSizeLimit"));
                }

                var token = $"{Guid.NewGuid()}.{fileInfo.Extension}";
                await _fileStorageManager.UploadTempFile(token, file.OpenReadStream().GetAllBytes());

                return Json(new AjaxResponse(new { fileName = file.FileName, fileToken = token, fileExtension = fileInfo.Extension }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        /******** DownloadTempFile Original *********/
        [DisableAuditing]
        public ActionResult DownloadTempFileOriginal(FileDto file)
        {
            #region original one
            var fileBytes = _tempFileCacheManager.GetFile(file.FileToken);
            if (fileBytes == null)
            {
                return NotFound(L("RequestedFileDoesNotExists"));
            }

            return File(fileBytes, file.FileType, file.FileName);
            #endregion
        }

        private string ConvertExtentionToContentType(string extension)
        {
            switch (extension)
            {
                case "pdf":
                    return "application/pdf";
                case "xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case "txt":
                    return "text/plain"; 
                default:
                    return "";
            }
        }

        [DisableAuditing]
        [AbpMvcAuthorize]
        public async Task<JsonResult> ImportExcelFile()
        {
            try
            {
                //Check input
                if (Request.Form.Files.Count <= 0 || Request.Form.Files[0] == null)
                {
                    throw new UserFriendlyException(L("ImportFileError"));
                }
                var file = Request.Form.Files[0];
                //Guid guid = Guid.NewGuid();
                var fileInfo = new FileInfo(file.FileName);

                if (file.Length > (5242880 * 10)) //1MB * 10.
                {
                    throw new UserFriendlyException(L("ImportFileExcelWarnSizeLimit"));
                }

                //Check file type & format
                if (fileInfo.Extension.ToLower().Trim() != ".xlsx" && fileInfo.Extension.ToLower().Trim() != ".xls")
                {
                    throw new UserFriendlyException(L("ImportFileWarnExcel"));
                }

                //var tempFilePath = Path.Combine(_appFolders.TempFileDownloadFolder, guid.ToString());

                //using (var stream = new FileStream(tempFilePath, FileMode.Create))
                //{
                //    file.CopyTo(stream);
                //}
                ////file.CopyTo(tempFilePath);

                var token = $"{Guid.NewGuid()}.{fileInfo.Extension}";
                await _fileStorageManager.UploadTempFile(token, file.OpenReadStream().GetAllBytes());

                return Json(new AjaxResponse(new { fileName = file.FileName, fileToken = token, fileExtension = fileInfo.Extension }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [DisableAuditing]
        public async Task<ActionResult> DownloadBinaryFile(Guid id, string contentType, string fileName)
        {
            var fileObject = await _binaryObjectManager.GetOrNullAsync(id);
            if (fileObject == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }

            return File(fileObject.Bytes, contentType, fileName);
        }
    }
}