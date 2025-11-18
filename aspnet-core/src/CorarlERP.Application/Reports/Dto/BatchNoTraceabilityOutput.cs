using Abp.Application.Services.Dto;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{
    public class BatchNoTraceabilityBalanceOutput
    {

        public IReadOnlyList<BatchNoTraceabilityOutput> ReceiptItems { get;set; }
        public IReadOnlyList<BatchNoTraceabilityOutput> IssueItems { get;set; }
        public IReadOnlyList<ProductionPlanBatchOutput> ProductionPlanReceipts { get; set; }
        public IReadOnlyList<ProductionPlanBatchOutput> ProductionPlanIssues { get; set; }
        public string BatchNumber { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public decimal NetWeight { get; set; }
        public decimal ReceiptQty { get; set; }
        public decimal IssueQty { get; set; }
        public decimal BalanceQty { get; set; }
    }

    public class ProductionPlanBatchOutput
    {

        public Guid? ProductioinId { get; set; }
        public string ProductionPlanNo { get; set; }
        public decimal ReceiptQty { get; set; }
        public decimal IssueQty { get; set; }
        public decimal BalanceQty { get; set; }
        public IReadOnlyList<BatchNoTraceabilityOutput> ProductionReceiptItems { get; set; }
        public IReadOnlyList<BatchNoTraceabilityOutput> ProductionIssueItems { get; set; }
    }

    public class BatchNoTraceabilityOutput
    {
        public JournalType? JournalType { get; set; }
        public string JournalTypeName { get; set; }
        public Guid TransactionId { get; set; }
        public DateTime Date { get; set; }
        public string JournalNo { get; set; }
        public string Reference { get; set; }
        public string PartnerName { get; set; }
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public Guid BatchNoId { get; set; }
        public string BatchNumber { get; set; }
        public decimal Qty { get; set; }
        public Guid? TransferId { get; set; }
        public Guid? ProductioinId { get; set; }
        public string ProductionPlanNo { get; set; }
        public Guid? ProductionPlanId { get; set; }
        public bool IsReceipt { get; set; }
        public string Unit { get; set; }
        public decimal NetWeight { get; set; }
    }

    public class MoreBatchNoOutput
    {
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public Guid BatchNoId { get; set; }
        public string BatchNumber { get; set; }
        public decimal Qty { get; set; }
        public Guid? ProductioinId { get; set; }
        public bool IsReceipt { get; set; }
        public string Unit { get; set; }
        public decimal NetWeight { get; set; }
    }

    public class FindBatchNoTraceabilityOutput
    {
        public Guid ItemIssueId { get; set; }
        public string ItemIssueNo { get; set; }
        public string Reference { get; set; }
        public DateTime Date { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public Guid BatchNoId { get; set; }
        public string BatchNumber { get; set; }
        public decimal Qty { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceRef { get; set; }
    }

    public class TraceabilityInvoiceOutput
    {
        public Guid ItemIssueId { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceRef { get; set; }
    }
}
