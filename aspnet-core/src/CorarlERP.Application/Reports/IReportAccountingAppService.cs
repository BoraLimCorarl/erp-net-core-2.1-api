using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.AccountTransactions;
using CorarlERP.Common.Dto;
using CorarlERP.Dto;
using CorarlERP.Reports.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Reports
{
    public interface IReportAccountingAppService : IApplicationService
    {

        Task<List<GetListIncomeReportOutput>> GetIncomeReport(GetIncomeInput input);
        ReportOutput GetReportTemplateIncome();
        Task<FileDto> ExportExcelIncomeReport(GetIncomeReportInput input);
        Task<FileDto> ExportPdfIncomeReport(GetIncomeReportInput input);
        

        Task<List<MainAccountGroupHeader>> GetBalanceSheetReport(GetBalanceSheetInput input);
        ReportOutput GetReportTemplateBalanceSheet();
        Task<FileDto> ExportExcelBalanceSheetReport(GetBalanceSheetReportInput input);
        Task<FileDto> ExportPdfBalanceSheetReport(GetBalanceSheetReportInput input);


        Task<PagedResultWithTotalColuumnsDto<JournalReportOutput>> GetListJournalReport(GetListJournalReportInput input);
        Task<ReportOutput> GetReportTemplateJournal();
        Task<FileDto> ExportExcelJournalReport(JournalExportReportInput input);
        Task<FileDto> ExportPDFJournalReport(JournalExportReportInput input);


        Task<PagedResultWithTotalColuumnsDto<AccountTransactionOutput>> GetListLedgerReport(GetListLedgerReportInput input);
        Task<FileDto> ExportExcelLedgerReport(LedgerExportReportInput input);
        Task<FileDto> ExportPDFLedgerReport(LedgerExportReportInput input);


        Task<PagedResultWithTotalColuumnsDto<CashReportOutput>> GetListCashReport(GetListCashReportInput input);
        ReportOutput GetCashReportTemplate();
        Task<FileDto> ExportExcelCashReport(ExportCashReportInput input);
        Task<FileDto> ExportPDFCashReport(ExportCashReportInput input);

        Task<IndirectCashFlowReportResultOutput> GetListCashFlowReport(GetListCashFlowReportInput input);
        ReportOutput GetCashFlowReportTemplate();
        Task<FileDto> ExportExcelCashFlowReport(ExportCashFlowReportInput input);
        Task<FileDto> ExportPDFCashFlowReport(ExportCashFlowReportInput input);

        Task<CashFlowReportResultOutput> GetListDirectCashFlowReport(GetListDirectCashFlowReportInput input);
        ReportOutput GetDirectCashFlowReportTemplate();
        Task<FileDto> ExportExcelDirectCashFlowReport(ExportDirectCashFlowReportInput input);
        Task<FileDto> ExportPDFDirectCashFlowReport(ExportDirectCashFlowReportInput input);

        Task<TrialBalanceReportResultOutput> GetTrialBalanceReport(GetTrialBalanceInput input);
        ReportOutput GetReportTemplateTrialBalance();
        Task<FileDto> ExportExcelTrialBalanceReport(GetTrialBalanceReportInput input);
        Task<FileDto> ExportPdfTrialBalanceReport(GetTrialBalanceReportInput input);

    }
}
