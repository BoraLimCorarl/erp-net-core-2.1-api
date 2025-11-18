using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.CustomerManager;
using CorarlERP.Dto;
using CorarlERP.Reports.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Reports
{
    public interface IReportCustomerAppService : IApplicationService
    {

        Task<PagedResultWithTotalColuumnsDto<GetListSaleInvoiceReportOutput>> GetSaleInvoiceReport(GetListReportSaleInvoiceInput input);
        ReportOutput GetReportTemplateSaleInvoice();
        Task<FileDto> ExportExcelSaleInvoiceReport(GetSaleInvoiceReportInput input);
        Task<FileDto> ExportPDFSaleInvoiceReport(GetSaleInvoiceReportInput input);


        Task<PagedResultWithTotalColuumnsDto<GetListCustomerAgingReportOutput>> GetCustomerAgingReport(GetListCustomerAgingInput input);
        ReportOutput GetReportTemplateCustomerAging();
        Task<FileDto> ExportExcelCustomerAgingReport(GetCustomerAgingReportInput input);
        Task<FileDto> ExportPDFCustomerAgingReport(GetCustomerAgingReportInput input);


        Task<PagedResultWithTotalColuumnsDto<GetCustomerByInvoiceReportOutput>> GetCustomerByInvoiceReport(GetListCustomerAgingInput input);
        ReportOutput GetReportTemplateCustomerByInvoice();
        Task<FileDto> ExportExcelCustomerByInvoiceReport(GetCustomerAgingReportInput input);
        Task<FileDto> ExportPDFCustomerByInvoiceReport(GetCustomerAgingReportInput input);

        Task<PagedResultWithTotalColuumnsDto<GetListSaleReturnReportOutput>> GetSaleReturnReport(GetSaleReturnReportInput input);
        Task<FileDto> ExportExcelSaleReturnReport(GetSaleReturnReportInput input);
        Task<FileDto> ExportPDFSaleReturnReport(GetSaleReturnReportInput input);

        Task<PagedResultWithTotalColuumnsDto<GetListSaleInvoiceDetailReportOutput>> GetSaleInvoiceDetailReport(GetListReportSaleInvoiceInput input);
        ReportOutput GetReportTemplateSaleInvoiceDetail();
        Task<FileDto> ExportExcelSaleInvoiceDetailReport(GetSaleInvoiceDetailReportInput input);
        Task<FileDto> ExportPDFSaleInvoiceDetailReport(GetSaleInvoiceDetailReportInput input);

        Task<PagedResultWithTotalColuumnsDto<GetCustomerByInvoiceWithPaymentReportOutput>> GetCustomerByInvoiceWithPaymentReport(GetListCustomerAgingInput input);
        ReportOutput GetReportTemplateCustomerByInvoiceWithPayment();
        Task<FileDto> ExportExcelCustomerByInvoiceWithPaymentReport(GetCustomerAgingReportInput input);
        Task<FileDto> ExportPDFCustomerByInvoiceWithPaymentReport(GetCustomerAgingReportInput input);

        ReportOutput GetReportTemplateSaleOrderSummary();
        Task<PagedResultWithTotalColuumnsDto<GetListReportSaleOrderSummaryOutput>> GetSaleOrderSummaryReport(GetListReportSaleOrderSummaryInput input);
        Task<FileDto> ExportExcelSaleOrderSummaryReport(GetSaleOrderSummaryReportInput input);
        Task<FileDto> ExportPDFSaleOrderSummaryReport(GetSaleOrderSummaryReportInput input);

        Task<ReportOutput> GetReportTemplateSaleOrderDetail();
        Task<PagedResultWithTotalColuumnsDto<GetListReportSaleOrderDetailOutput>> GetSaleOrderDetailReport(GetListReportSaleOrderSummaryInput input);
        Task<FileDto> ExportExcelSaleOrderDetailReport(GetSaleOrderSummaryReportInput input);
        Task<FileDto> ExportPDFSaleOrderDetailReport(GetSaleOrderSummaryReportInput input);

        Task<ReportOutput> GetReportTemplateProfitByInvoice();
        Task<PagedResultWithTotalColuumnsDto<GetListReportProfitByInvoiceOutput>> GetProfitByInvoiceReport(GetListReportProfitByInvoiceInput input);
        Task<FileDto> ExportExcelProfitByInvoiceReport(GetProfitByInvoiceReportInput input);
        Task<FileDto> ExportPDFProfitByInvoiceReport(GetProfitByInvoiceReportInput input);



        Task<PagedResultWithTotalColuumnsDto<GetListReportDeliveryScheduleDetailOutput>> GetDeliveryScheduleDetailReport(GetListReportDeliveryScheduleSummaryInput input);
        Task<ReportOutput> GetReportTemplateDeliveryScheduleDetail();
        Task<FileDto> ExportExcelDeliveryScheduleDetailReport(GetDeliveryScheduleSummaryReportInput input);
        Task<FileDto> ExportPDFDeliveryScheduleDetailReport(GetDeliveryScheduleSummaryReportInput input);



        ReportOutput GetReportTemplateDeliveryScheduleSummary();
        Task<PagedResultWithTotalColuumnsDto<GetListReportDeliveryScheduleSummaryOutput>> GetDeliveryScheduleSummaryReport(GetListReportDeliveryScheduleSummaryInput input);
        Task<FileDto> ExportExcelDeliveryScheduleSummaryReport(GetDeliveryScheduleSummaryReportInput input);
        Task<FileDto> ExportPDFDeliveryScheduleSummaryReport(GetDeliveryScheduleSummaryReportInput input);

        Task<ReportOutput> GetReportTemplateSaleOrderByItemProperty();
        Task<PagedResultWithTotalColuumnsDto<GetListReportSaleOrderByItemPropertyOutput>> GetSaleOrderByItemPropertyReport(GetListReportSaleOrderByItemPropertyInput input);
        Task<FileDto> ExportExcelSaleOrderByItemPropertyReport(GetSaleOrderByItemPropertyReportInput input);
        Task<FileDto> ExportPDFSaleOrderByItemPropertyReport(GetSaleOrderByItemPropertyReportInput input);
        
        Task<ReportOutput> GetReportTemplateDeliveryScheduleByItemProperty();
        Task<DeliveryScheduleReportResultOutput> GetDeliveryScheduleByItemPropertyReport(GetListReportDeliveryScheduleByItemPropertyInput input);
        Task<FileDto> ExportExcelDeliveryScheduleByItemPropertyReport(GetDeliveryScheduleByItemPropertyReportInput input);
        Task<FileDto> ExportPDFDeliveryScheduleByItemPropertyReport(GetDeliveryScheduleByItemPropertyReportInput input);

        Task<PagedResultWithTotalColuumnsDto<GetListSaleInvoiceByItemPropertyReportOutput>> GetSaleInvoiceByItemPropertyReport(GetListReportSaleInvoiceByItemPropertyInput input);
        Task<ReportOutput> GetReportTemplateSaleInvoiceByItemProperty();
        Task<FileDto> ExportExcelSaleInvoiceByItemPropertyReport(GetSaleInvoiceByItemPropertyReportInput input);
        Task<FileDto> ExportPDFSaleInvoiceByItemPropertyReport(GetSaleInvoiceByItemPropertyReportInput input);

    }
}
