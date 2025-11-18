using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CorarlERP.Galleries
{
    public class GalleryDownloadOutput
    {
        public Stream Stream { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}
