using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Productions.Dto
{
   public class CreateProductionInput
    {
        public TransactionStatus Status { get; set; }       
        public string ProductionNo { get; set; }
        public DateTime Date { get; set; }
        public string Reference { get; set; }
        public long FromLocationId { get; set; }
        public long ToLocationId { get; set; }
        public long FromClassId { get; set; }
        public long ToClassId { get; set; }
        public string Memo { get; set; }
        public List<CreateOrUpdateFinishItemInput> FinishItems { get; set; }
        public List<CreateOrUpdateRawMaterialItemsInput> RawMaterialItems { get; set; }
        public bool ConvertToIssueAndReceipt { get; set; }
        public DateTime? ItemReceiptDate { get; set; }
        public DateTime? ItemIssueDate { get; set; }
        public decimal SubTotalRawProduction { get; set; }
        public decimal SubTotalFinishProduction { get; set; }

        [Required]
        public Guid ProductionAccountId { get; set; }
        public long ProductionProcessId { get; set; }

        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }
        public Guid? ProductionPlanId { get; set; }

        public decimal TotalIssueQty { get; set; }
        public decimal TotalReceiptQty { get; set; }
        public decimal TotalIssueNetWeight { get; set; }
        public decimal TotalReceiptNetWeight { get; set; }

        public List<StandardGroupSummary> StandardGroups { get; set; }
        public List<StandardGroupSummary> IssueStandardGroups { get; set; }

    }

    
}
