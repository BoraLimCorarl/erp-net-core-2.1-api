using Abp.AspNetZeroCore.Net;
using Abp.Dependency;
using CorarlERP.Dto;
using NReco.PdfGenerator;
using System.IO;

namespace CorarlERP.Reports.PDF
{

    //PDF
    public class HtmlToPdfBase : ReportBaseClass, ITransientDependency
    {
        #region Initialization
        public object HttpContextServer { get; private set; }

        private readonly string _fileName;

        private readonly PageOrientation _orientation;


        private readonly int? _marginBottom;

        private readonly int? _marginTop;

        private readonly int? _marginLeft;

        private readonly int? _marginRight;

        private readonly float? _zoom;
        private readonly int? _pageHeight;
        private readonly int? _pageWidth;
        private readonly PageSize _pageSize;

        public HtmlToPdfBase(string fileName,
            PageOrientation orientation,
            int? marginBottom,
            int? marginTop,
            int? marginLeft,
            int? marginRight,
            float? zoom,
            int? pageHeight,
            int? pageWidth,
            PageSize pageSize)
        {
            _fileName = fileName;
            _orientation = orientation;
            _marginBottom = marginBottom;
            _marginTop = marginTop;
            _marginLeft = marginLeft;
            _marginRight = marginRight;
            _zoom = zoom;
            _pageHeight = pageHeight;
            _pageWidth = pageWidth;
            _pageSize = pageSize;
        }
        #endregion

        /// <summary>
        /// Function to export HTML to pdf
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public FileDto ExecuteResult(string html)
        {

            #region Nreco Library for Html to pdf conversion

            //#region font face applied  to the html
            //html = "<html>" +
            //           "<head>" +
            //                 "<link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css'>" +
            //                 "<link rel='stylesheet' href='https://fonts.googleapis.com/css?family=Khmer'>" +
            //                 "<meta charset='UTF-8'/>" +
            //                 "<style>" +
            //                   "body {font-family: 'Khmer'; font-size: 22px;}" +
            //                    "tr{page-break-inside: avoid;}" +
            //                   ".container { page-break-inside: avoid; page-break-before:always; position: relative;}" +
            //                 "</style>" +
            //           "</head>" +
            //              "<body>" +
            //                   html +
            //              "</body>" +
            //       "</html>";
            //#endregion

            var file = new FileDto(string.Format("{0}.pdf", _fileName), MimeTypeNames.ApplicationPdf);
            HtmlToPdfConverter pdfConverter = new HtmlToPdfConverter();


            #region Set page size
            if (_pageHeight == null && _pageWidth == null)
            {
                pdfConverter.Size = _pageSize;
            }
            else
            {
                pdfConverter.PageHeight = _pageHeight;
                pdfConverter.PageWidth = _pageWidth;
            }
            #endregion
            // Set page margin
            #region Margins
            var margins = new PageMargins();
            margins.Bottom = _marginBottom ?? 5;
            margins.Top = _marginTop ?? 5;
            margins.Left = _marginLeft ?? 5;
            margins.Right = _marginRight ?? 5;
            pdfConverter.Margins = margins;
            pdfConverter.Zoom = _zoom ?? 1;
            pdfConverter.TocHeaderText = "";

            #endregion
            //Page Orientation
            pdfConverter.Orientation = _orientation;

            //Html to Pdf Converter
            var pdfBytes = pdfConverter.GeneratePdf(html);

            var path = Path.Combine(AppFolders.TempFileDownloadFolder, file.FileToken);

            File.WriteAllBytes(path, pdfBytes);
            return file;



            #endregion


        }

        
    }
}
