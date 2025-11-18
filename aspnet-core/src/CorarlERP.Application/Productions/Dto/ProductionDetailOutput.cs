using Abp.AutoMapper;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.ProductionProcesses.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.Common.Dto.EnumStatus;

namespace CorarlERP.Productions.Dto
{
    [AutoMapFrom(typeof(Production))]
    public class ProductionDetailOutput
    {
        public Guid Id { get; set; }
        public Guid ProductionAccountId { get; set; }
        public ChartAccountSummaryOutput ProductionAccount { get; set; }
        public string ProductionNo { get; set; }
        public DateTime Date { get; set; }
        public string Reference { get; set; }
        public TransactionStatus Status { get; set; }
        public TransferStatus ShipedStatus { get; set; }

        public long ToLocationId { get; set; }
        public LocationSummaryOutput ToLocation { get; set; }
        public long FromLocationId { get; set; }
        public LocationSummaryOutput FromLocation { get; set; }


        public long ToClassId { get; set; }
        public ClassSummaryOutput ToClass { get; set; }
        public long FromClassId { get; set; }
        public ClassSummaryOutput FromClass { get; set; }

        public string Memo { get; set; }
        public List<CreateOrUpdateFinishItemInput> FinishItems { get; set; }
        public List<CreateOrUpdateRawMaterialItemsInput> RawMaterialItems { get; set; }
        public bool ConvertToIssueAndReceipt { get; set; }
        public DateTime? ItemReceiptDate { get; set; }
        public DateTime? ItemIssueDate { get; set; }
        public Guid? ItemReceiptId { get; set; }
        public Guid? ItemIssueId { get; set; }
        public decimal SubTotalRawProduction { get; set; }
        public decimal SubTotalFinishProduction { get; set; }


        public long ProductionProcessId { get; set; }
        public ProductionProcessSummaryOutput ProductionProcess { get; set; }
        public Guid? ProductionPlanId { get; set; }
        public string ProductionPlanNo { get; set; }

        public decimal TotalIssueQty { get; set; }
        public decimal TotalReceiptQty { get; set; }
        public decimal TotalIssueNetWeight { get; set; }
        public decimal TotalReceiptNetWeight { get; set; }
    }
}
