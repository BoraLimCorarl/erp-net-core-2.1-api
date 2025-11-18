using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Classes;
using CorarlERP.Locations;
using CorarlERP.ProductionPlans;
using CorarlERP.ProductionProcesses;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Productions
{
    public enum CalculationState
    {
        NotSync = 0,
        Synced = 1,
        Calculated = 2,
        CalculateErr = 3,
        CalculateAdj = 4
    }

    [Table("CarlErpTransProductions")]
    public class Production : BaseAuditedEntity<Guid>
    {
        public const int MaxOrderNumberLength = 128;
        [Required]
        [MaxLength(MaxOrderNumberLength)]
        public string ProductionNo { get; private set; }
        [MaxLength(MaxOrderNumberLength)]
        public string Reference { get;private set; }
        public DateTime Date { get; private set; }
        public Guid ProductionAccountId { get; private set; }
        public ChartOfAccount ProductionAccount { get; private set; }


        public long? ProductionProcessId { get; private set; }
        public ProductionProcess ProductionProcess { get; private set; }

        public Guid? ProductionPlanId { get; private set; }
        public ProductionPlan ProductionPlan { get; private set; }

        [Required]
        public long ToLocationId { get; private set; }
        public Location ToLocation { get; private set; }
        [Required]
        public long FromLocationId { get; private set; }
        public Location FromLocation { get; private set; }
        [Required]
        public long ToClassId { get; private set; }
        public Class ToClass { get; private set; }
        [Required]
        public long FromClassId { get; private set; }
        public Class FromClass { get; private set; }

        public void SetFromClass(long classId) => FromClassId = classId;
        public void SetToClass(long classId) => ToClassId = classId;
        public void SetFromLocation(long locationId) => FromLocationId = locationId;
        public void SetToLocation(long locationId) => ToLocationId = locationId;


        public string Memo { get; private set; }
        public TransferStatus ShipedStatus { get; private set; }
        public TransactionStatus Status { get; private set; }

        public bool ConvertToIssueAndReceipt { get; private set; }
        public DateTime? ReceiptDate { get; private set; }
        public DateTime? IssueDate { get; private set; }

        public decimal SubTotalRawProduction { get; private set; }
        public decimal SubTotalFinishProduction { get; private set; }

        public decimal TotalIssueQty { get; private set; }  
        public decimal TotalReceiptQty { get; private set; }
        public decimal TotalIssueNetWeight { get; private set; }
        public decimal TotalReceiptNetWeight { get; private set; }
        public void SetDate(DateTime date) => Date = date;
        public void SetReceiptDate(DateTime? receiptDate) => ReceiptDate = receiptDate;
        public void SetIssueDate(DateTime? issueDate) => IssueDate = issueDate;

        public void UpdateSummary(decimal totalIssueQty, decimal totalReceiptQty, decimal totalIssueNetWeight, decimal totalReceiptNetWeight)
        {
            TotalIssueQty = totalIssueQty;
            TotalReceiptQty = totalReceiptQty;
            TotalIssueNetWeight = totalIssueNetWeight;
            TotalReceiptNetWeight = totalReceiptNetWeight;
        }

        public void UpdateStatus (TransactionStatus status)
        {
            Status = status;
        }
        public void UpdateShipedStatus(TransferStatus shipStatus)
        {
            ShipedStatus = shipStatus;
        }

        public CalculationState CalculationState { get; private set; }
        public void SetCalculateionState(CalculationState calculationState) => CalculationState = calculationState; 

        public static Production Create(
           int? tenantId,
           long creatorUserId,
           long locationFromId,
           long locationToId,
           long classFromId,
           long classToId,
           string productionNo,
           DateTime date,
           string reference,
           TransactionStatus status,
           string memo,
           bool convertToIssueAndReceipt,
           DateTime? receiptDate,
           DateTime? issueDate,
           decimal subTotalRawProduction,
           decimal subTotalFinishProduction,
           Guid productionAccountId,
           long productionProcessId,
           Guid? productionPlanId,
           decimal totalIssueQty,
           decimal totalReceiptQty,
           decimal totalIssueNetWeight,
           decimal totalReceiptNetWeight 
           )
        {
            return new Production()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ProductionAccountId = productionAccountId,
                ProductionProcessId = productionProcessId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                Memo = memo,
                Status = status,
                FromLocationId = locationFromId,
                ToLocationId = locationToId,
                FromClassId = classFromId,
                ToClassId = classToId,
                ProductionNo = productionNo,
                Date = date,
                Reference = reference,
                ShipedStatus = TransferStatus.Pending,
                ConvertToIssueAndReceipt = convertToIssueAndReceipt,
                IssueDate = issueDate,
                ReceiptDate = receiptDate,
                SubTotalRawProduction = subTotalRawProduction,
                SubTotalFinishProduction = subTotalFinishProduction,
                ProductionPlanId = productionPlanId,
                TotalIssueQty = totalIssueQty,
                TotalReceiptQty = totalReceiptQty,
                TotalIssueNetWeight = totalIssueNetWeight,
                TotalReceiptNetWeight = totalReceiptNetWeight
            };
        }

        public void Update(
           long lastModifiedUserId,
           long locationFromId,
           long locationToId,
           long classFromId,
           long classToId,
           TransactionStatus status,
           string productionNo,
           DateTime date,
           string reference,
           string memo,
           bool convertToIssueAndReceipt,
           DateTime? receiptDate,
           DateTime? issueDate,
           decimal subTotalRawProduction,
           decimal subTotalFinishProduction,
           Guid productionAccountId,
           long productionProcessId,
           Guid? productionPlanId,
           decimal totalIssueQty,
           decimal totalReceiptQty,
           decimal totalIssueNetWeight,
           decimal totalReceiptNetWeight)
        {
            LastModifierUserId = lastModifiedUserId;
            ProductionAccountId = productionAccountId;
            ProductionProcessId = productionProcessId;
            LastModificationTime = Clock.Now;
            Memo = memo;
            Status = status;
            FromLocationId = locationFromId;
            ToLocationId = locationToId;
            FromClassId = classFromId;
            ToClassId = classToId;
            ProductionNo = productionNo;
            Date = date;
            Reference = reference;
            ConvertToIssueAndReceipt = convertToIssueAndReceipt;
            ReceiptDate = receiptDate;
            IssueDate = issueDate;
            SubTotalFinishProduction = subTotalFinishProduction;
            SubTotalRawProduction = subTotalRawProduction;
            ProductionPlanId = productionPlanId;
            TotalIssueQty = totalIssueQty;
            TotalReceiptQty = totalReceiptQty;
            TotalIssueNetWeight = totalIssueNetWeight;
            TotalReceiptNetWeight = totalReceiptNetWeight;
        }


        public void UpdateTotalRaw(decimal subTotalRawProduction)
        {
            SubTotalRawProduction = subTotalRawProduction;
        }


    }
}
