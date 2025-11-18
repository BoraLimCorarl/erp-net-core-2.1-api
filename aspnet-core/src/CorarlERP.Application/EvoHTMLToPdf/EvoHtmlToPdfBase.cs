using Abp.AspNetZeroCore.Net;
using Abp.Dependency;
using CorarlERP;
using CorarlERP.Dto;
using EvoPdf.HtmlToPdfClient;
using Newtonsoft.Json.Linq;
using NReco.PdfGenerator;
using System.IO;

namespace CarlPayroll.Pdf.Select.ExportPdfFromHtml
{
    public class PayrollHtmlToPdfBase : CorarlERPAppServiceBase, ITransientDependency
    {
        public EvoPdf.HtmlToPdfClient.HtmlToPdfConverter GetInitPDFOption()
        {
            var _evoLicenseKey = GetEvoLicenKey();

            EvoPdf.HtmlToPdfClient.HtmlToPdfConverter htmlToPdfConverter = new EvoPdf.HtmlToPdfClient.HtmlToPdfConverter();
            htmlToPdfConverter.LicenseKey = _evoLicenseKey.ToString();
            htmlToPdfConverter.PdfDocumentOptions.EmbedFonts = true;
            htmlToPdfConverter.PdfDocumentOptions.FitWidth = true;
            htmlToPdfConverter.PdfDocumentOptions.TableHeaderRepeatEnabled = false;
            htmlToPdfConverter.PdfDocumentOptions.ShowHeader = true;
            htmlToPdfConverter.PdfDocumentOptions.ShowFooter = true;
            htmlToPdfConverter.PdfDocumentOptions.X = 30;
            htmlToPdfConverter.PdfDocumentOptions.Y = 10;
            htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Landscape;
            htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
            return htmlToPdfConverter;
        }
        
    }

}












