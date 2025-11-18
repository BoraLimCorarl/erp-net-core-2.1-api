using Abp.Web.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Galleries
{
    public class UploadGalleryFileOutput 
    {
        public string FileName { get; set; }

        public string FileType { get; set; }
        public string FileSize { get; set; }

        public Guid GalleryId { get; set; }

    }
}
