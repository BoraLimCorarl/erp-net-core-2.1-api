using System;
using System.ComponentModel.DataAnnotations;
using CorarlERP.Galleries;

namespace CorarlERP.Dto
{
    public class FileDto
    {
        [Required]
        public string FileName { get; set; }

        public string FileType { get; set; }

        public string FileToken { get; set; }

        public string FileUrl { get; set; }

        public string SubFolder { get; set; }

        public FileDto()
        {
            
        }

        public FileDto(string fileName, string fileType)
        {
            FileName = fileName;
            FileType = fileType;
            FileToken = Guid.NewGuid().ToString("N");
        }
    }

    public class GalleryFileDto: FileDto
    {
        [Required]
        public UploadFrom UploadFrom { get; set; }
    }

}