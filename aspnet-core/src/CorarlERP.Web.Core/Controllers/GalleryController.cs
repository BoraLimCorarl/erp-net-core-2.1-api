using Abp.AspNetCore.Mvc.Authorization;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp.UI;
using Abp.Web.Models;
using ByteSizeLib;
using CorarlERP.Dto;
using CorarlERP.FileUploads;
using CorarlERP.Galleries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CorarlERP.Web.Controllers
{
    public class GalleryController : CorarlERPControllerBase
    {

        private readonly IFileUploadManager _fileUploadManager;
        public GalleryController(
            IFileUploadManager fileUploadManager
            )
        {
            _fileUploadManager = fileUploadManager;
        }

        private bool CheckFileType(string contentType)
        {
            var supportedTypes = new[] { "text/html", "application/pdf", "image/jpeg", "image/png", "image/jpg", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };
            if (supportedTypes.Contains(contentType))
            {
                return true;
            }
            return false;
            //string ext = Path.GetExtension(fileName);
            //switch (ext.ToLower())
            //{
            // case ".gif":
            // return true;
            // case ".jpg":
            // return true;
            // case ".jpeg":
            // return true;
            // case ".png":
            // return true;
            // case ".pdf":
            // return true;
            // case ".doc":
            // return true;
            // case ".docx":
            // return true;
            // default:
            // return false;
            //}
        }

        [AbpMvcAuthorize]
        [DisableAuditing]
        [UnitOfWork(IsDisabled = true)]
        [AbpAuthorize]
        public async Task<UploadGalleryFileOutput> Upload(GalleryFileDto input)
        {
            var file = Request.Form.Files.First();

            //Check input                
            if (file == null)
            {
                throw new Abp.UI.UserFriendlyException(L("IsNotValid", L("File")));
            }

            if (file.Length > CorarlERPConsts.MaxGalleryFileSize)
            {
                throw new UserFriendlyException(L("FileSizeLimit", CorarlERPConsts.MaxGalleryFileSize));
            }

            if (!CheckFileType(input.FileType))
            {
                throw new Abp.UI.UserFriendlyException(L("IsNotValid", L("FileType")));
            }

            try
            {              

                return await _fileUploadManager.Upload(AbpSession.TenantId, AbpSession.UserId, input.UploadFrom, file, input.FileType);
            }
            catch (UserFriendlyException ex)
            {
                throw new Abp.UI.UserFriendlyException(L(ex.Message));
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(L("CannotUpload"));
            }
            
        }

        [DisableAuditing]
        [UnitOfWork(IsDisabled = true)]
        [AbpAuthorize]
        public async Task<ActionResult> Index(Guid id)
        {
            try
            {
                var galleryFile = await _fileUploadManager.DownLoad(AbpSession.TenantId, id);

                return File(galleryFile.Stream, galleryFile.ContentType, fileDownloadName: galleryFile.FileName);
            }
            catch(UserFriendlyException ex)
            {
                throw new UserFriendlyException(L(ex.Message));
            }
            catch(Exception ex)
            {
                throw new UserFriendlyException(L("FileNotFound"));
            }
        }

    }
}
