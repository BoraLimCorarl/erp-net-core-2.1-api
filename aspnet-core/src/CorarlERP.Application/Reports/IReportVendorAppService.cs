using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Dto;
using CorarlERP.Reports.Dto;
using CorarlERP.VendorHelpers.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Reports
{
    public interface IReportVendorAppService : IApplicationService
    {

        Task<PagedResultWithTotalColuumnsDto<GetListReportBillOutput>> GetBillReport(GetListReportBillInput input);
        ReportOutput GetReportTemplateBill();
        Task<FileDto> ExportExcelBillReport(GetBillReportInput input);
        Task<FileDto> ExportPDFBillReport(GetBillReportInput input);



        Task<PagedResultWithTotalColuumnsDto<GetListVendorAgingReportOutput>> GetVendorAgingReport(GetListVendorAgingInput input);
        ReportOutput GetReportTemplateVendorAging();
        Task<FileDto> ExportExcelVendorAgingReport(GetVendorAgingReportInput input);
        Task<FileDto> ExportPDFVendorAgingReport(GetVendorAgingReportInput input);



        Task<PagedResultWithTotalColuumnsDto<GetVendorByBillReportOutput>> GetVendorByBillReport(GetListVendorAgingInput input);
        ReportOutput GetReportTemplateVendorByBill();
        Task<FileDto> ExportExcelVendorByBillReport(GetVendorAgingReportInput input);
        Task<FileDto> ExportPDFVendorByBillReport(GetVendorAgingReportInput input);

        Task<PagedResultWithTotalColuumnsDto<GetListPurchaseByItemPropertyReportOutput>> GetPurchaseByItemPropertyReport(GetListReportPurchaseByItemPropertyInput input);
        Task<ReportOutput> GetReportTemplatePurchaseByItemProperty();
        Task<FileDto> ExportExcelPurchaseByItemPropertyReport(GetPurchaseByItemPropertyReportInput input);
        Task<FileDto> ExportPDFPurchaseByItemPropertyReport(GetPurchaseByItemPropertyReportInput input);

        Task<PagedResultDto<GetListQCTestReportOutput>> GetQCTestReport(GetListReportQCTestInput input);
        Task<ReportOutput> GetReportTemplateQCTest();
        Task<FileDto> ExportExcelQCTestReport(GetQCTestReportInput input);
        Task<FileDto> ExportPDFQCTestReport(GetQCTestReportInput input);
    }
}
