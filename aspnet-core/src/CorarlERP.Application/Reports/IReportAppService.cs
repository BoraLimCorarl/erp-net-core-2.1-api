using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Dto;
using CorarlERP.Inventories.Data;
using CorarlERP.ProductionPlans.Dto;
using CorarlERP.Productions.Dto;
using CorarlERP.Reports.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Reports
{
    public interface IReportAppService : IApplicationService
    {
        Task<PagedResultWithTotalColuumnsDto<GetListInventoryReportOutputNew>> GetInventoryReport(GetInventoryInput input);
        ReportOutput GetReportTemplateInventory();
        Task<FileDto> ExportExcelInventoryReport(GetInventoryReportInput input);
        Task<FileDto> ExportPdfInventoryReport(GetInventoryReportInput input);
        

        Task<PagedResultDto<InventoryValuationDetailItem>> GetInventoryValuationDetailReport(GetListInventoryValuationInput input);
        Task<FileDto> ExportExcelInventoryValuationDetailReport(GetInventoryValuationDetailReportInput input);
        Task<FileDto> ExportPdfInventoryValuationDetailReport(GetInventoryValuationDetailReportInput input);


        Task<PagedResultWithTotalColuumnsDto<GetListInventoryTransationReportOutput>> GetInventoryTransactionReport(InventoryTransactionInput input);
        ReportOutput GetReportTemplateInventoryTransaction();
        Task<FileDto> ExportExcelInventoryTransactionReport(GetInventoryTransactionExportReportInput input);
        Task<FileDto> ExportPdfInventoryTransactionReport(GetInventoryTransactionExportReportInput input);

        Task<PagedResultWithTotalColuumnsDto<GetListStockBalanceReportOutputNew>> GetStockBalanceReport(GetStockBalanceInput input);
        ReportOutput GetReportTemplateStock();
        Task<FileDto> ExportExcelStockBalanceReport(GetStockBalanceReportInput input);
        Task<FileDto> ExportPdfStockBalanceReport(GetStockBalanceReportInput input);


        Task<PagedResultWithTotalColuumnsDto<GetListAssetBalanceReportOutput>> GetAssetBalanceReport(GetAssetBalanceInput input);
        ReportOutput GetReportTemplateAsset();
        Task<FileDto> ExportExcelAssetBalanceReport(GetAssetBalanceReportInput input);
        Task<FileDto> ExportPdfAssetBalanceReport(GetAssetBalanceReportInput input);

        Task<PagedResultWithTotalColuumnsDto<GetListStockBalanceReportOutputNew>> GetAssetItemDetailReport(GetStockBalanceInput input);
        ReportOutput GetReportTemplateAssetItemDetail();
        Task<FileDto> ExportExcelAssetItemDetailReport(GetStockBalanceReportInput input);
        Task<FileDto> ExportPdfAssetItemDetailReport(GetStockBalanceReportInput input);


        Task<BatchNoTraceabilityBalanceOutput> GetBatchNoTraceabilityReport(BatchNoTraceabilityInput input);
        Task<ListResultDto<MoreBatchNoOutput>> GetMoreBatchItems(GetMoreBatchItemInput input);
        Task<PagedResultDto<FindBatchNoTraceabilityOutput>> FindBatchNos(FindBatchNoTraceabilityInput input);

        Task<ReportOutput> GetReportTemplateBatchNoBalance();
        Task<PagedResultWithTotalColuumnsDto<BatchNoBalanceOutput>> GetBatchNoBalanceReport(BatchNoBalanceInput input);
        Task<FileDto> ExportExcelBatchNoBalanceReport(GetBatchNoBalanceReportInput input);
        Task<FileDto> ExportPdfBatchNoBalanceReport(GetBatchNoBalanceReportInput input);

        Task<ReportOutput> GetReportTemplateProduction();
        Task<PagedResultProductionSummaryDto<ProductionPlanDetailOutput>> GetProductionReport(ProductionReportInput input);
        Task<FileDto> ExportExcelProductionReport(GetProductionReportInput input);
        Task<FileDto> ExportPdfProductionReport(GetProductionReportInput input);
        Task Calculation(ProductionPlanCalculationInput input);


        Task<ReportOutput> GetReportTemplateProductionOrder();
        Task<PagedResultProductionSummaryDto<ProductionOrderReportOutput>> GetProductionOrderReport(ProductionOrderReportInput input);
        Task<FileDto> ExportExcelProductionOrderReport(GetProductionOrderReportInput input);
        Task<FileDto> ExportPdfProductionOrderReport(GetProductionOrderReportInput input);
        Task ProductionOrderCalculation(ProductionPlanCalculationInput input);

    }
}
