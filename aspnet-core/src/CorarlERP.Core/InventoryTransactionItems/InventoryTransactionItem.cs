using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CorarlERP.Items;
using CorarlERP.Journals;
using CorarlERP.Locations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.InventoryTransactionItems
{
    [Table("CarlErpInventoryTransactionItems")]
    public class InventoryTransactionItem : AuditedEntity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public Guid TransactionId { get; private set; }
        public Guid? TransferOrProductionId { get; private set; }
        public Guid? TransferOrProductionItemId { get; private set; }
        public Guid JournalId { get; private set; }
        public DateTime Date { get; private set; }
        public string OrderIndex { get; private set; } //{Date:yyyymmdd}-{CreationTimeIndex}-{CeationTime:yyMMddHHmmssfff}
        public JournalType JournalType { get; private set; }
        public string JournalRef { get; private set; }
        public string JournalNo { get; private set; }
        public long? CreationTimeIndex { get; private set; }

        public Guid ItemId { get; private set; }
        public Guid InventoryAccountId { get; private set; }
        public void SetInventoryAccount(Guid accountId) => InventoryAccountId = accountId;
        public long LocationId { get; private set; }
        public long LotId { get; private set; }
        public decimal Qty { get; private set; }
        public decimal UnitCost { get; private set; }
        public decimal LineCost { get; private set; }
        public decimal QtyOnHand { get; private set; }
        public decimal TotalCost { get; private set; }
        public decimal AvgCost { get; private set; }
        public decimal AdjustmentCost { get; private set; }
        public decimal LatestCost { get; private set; }
        public bool IsItemReceipt { get; private set; }
        public DateTime LastSyncTime { get; private set; }
        public string Description { get; private set; }

        public bool IsSameAs(InventoryTransactionItem input)
        {
            return this.ItemId == input.ItemId &&
                   this.Date == input.Date &&
                   this.LocationId == input.LocationId &&
                   this.LotId == input.LotId &&
                   this.JournalId == input.JournalId &&
                   this.JournalType == input.JournalType &&
                   this.JournalNo == input.JournalNo &&
                   this.JournalRef == input.JournalRef &&
                   this.CreationTimeIndex == input.CreationTimeIndex &&
                   this.OrderIndex == input.OrderIndex &&
                   this.TransactionId == input.TransactionId &&
                   this.TransferOrProductionId == input.TransferOrProductionId &&
                   this.TransferOrProductionItemId == input.TransferOrProductionItemId &&
                   this.CreatorUserId.Value == input.CreatorUserId.Value &&
                   this.CreationTime == input.CreationTime &&
                   this.LastModificationTime == input.LastModificationTime &&
                   this.LastModifierUserId == input.LastModifierUserId &&
                   this.Description == input.Description &&
                   this.Qty == input.Qty &&
                   (
                   !this.IsItemReceipt || 
                   this.JournalType == JournalType.ItemReceiptTransfer || 
                   this.JournalType == JournalType.ItemReceiptCustomerCredit || 
                   (this.UnitCost == input.UnitCost && this.TotalCost == input.TotalCost));
        }

        public void SetOrderIndex(DateTime date, long creationTimeIndex, DateTime creationTime, Guid id)
        {
            OrderIndex = $"{date:yyyyMMdd}-{creationTimeIndex}-{creationTime:yyyyMMddHHmmss-fffffff}-{id.ToString().ToUpper()}";
        }
        public static InventoryTransactionItem Create(
            int tenantId,
            long creatorUserId,
            DateTime creationTime,
            long? modifierUserId,
            DateTime? lastModificationTime,
            Guid id,
            Guid transactionId,
            Guid? transferOrProductionId,
            Guid? transferOrProductionItemId,
            Guid journalId,
            DateTime date,
            long creationTimeIndex,            
            JournalType journalType,
            string journalNo,
            string journalRef,
            Guid itemId,
            Guid inventoryAccountId,
            long locationId,
            long lotId,
            decimal qty,
            decimal unitCost,
            decimal lineCost,
            bool isItemReceipt,
            string description)
        {

            var result = new InventoryTransactionItem
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = creationTime,
                LastModifierUserId = modifierUserId,
                LastModificationTime = lastModificationTime,
                LastSyncTime = Abp.Timing.Clock.Now,
                Id = id,
                TransactionId = transactionId,
                TransferOrProductionId = transferOrProductionId,
                TransferOrProductionItemId = transferOrProductionItemId,
                JournalId = journalId,
                Date = date,
                JournalType = journalType,
                JournalNo = journalNo,
                JournalRef = journalRef,
                CreationTimeIndex = creationTimeIndex,
                ItemId = itemId,
                InventoryAccountId = inventoryAccountId,
                LocationId = locationId,
                LotId = lotId,
                Qty = qty,
                UnitCost = unitCost,
                LineCost = lineCost,
                IsItemReceipt = isItemReceipt,
                Description = description
            };

            result.SetOrderIndex(date, creationTimeIndex, creationTime, id);

            return result;
        }

        public void Update(
            long creatorUserId,
            DateTime creationTime,
            long? modifierUserId,
            DateTime? lastModificationTime,
            Guid id,
            Guid transactionId,
            Guid? transferOrProductionId,
            Guid? transferOrProductionItemId,
            Guid journalId,
            DateTime date,
            long creationTimeIndex,
            JournalType journalType,
            string journalNo,
            string journalRef,
            Guid itemId,
            Guid inventoryAccountId,
            long locationId,
            long lotId,
            decimal qty,
            decimal unitCost,
            decimal lineCost,
            bool isItemReceipt,
            string description)
        {
            CreatorUserId = creatorUserId;
            CreationTime = creationTime;
            LastModifierUserId = modifierUserId;
            LastModificationTime = lastModificationTime;
            LastSyncTime = Abp.Timing.Clock.Now;
            Id = id;
            TransactionId = transactionId;
            TransferOrProductionId = transferOrProductionId;
            TransferOrProductionItemId = transferOrProductionItemId;
            JournalId = journalId;
            JournalNo = journalNo;
            JournalRef = journalRef;
            CreationTimeIndex = creationTimeIndex;
            Date = date;
            JournalType = journalType;
            ItemId = itemId;
            InventoryAccountId = inventoryAccountId;
            LocationId = locationId;
            LotId = lotId;
            Qty = qty;
            UnitCost = unitCost;
            LineCost = lineCost;
            IsItemReceipt = isItemReceipt;
            Description = description;

            SetOrderIndex(date, creationTimeIndex, creationTime, id);
        }

        public void SetQty(decimal qty) => Qty = qty;
        public void SetUnitCost(decimal unitCost) => UnitCost = unitCost;
        public void SetLineCost(decimal lineCost) => LineCost = lineCost;
        public void SetQtyOnHand(decimal qtyOnHand) => QtyOnHand = qtyOnHand;
        public void SetTotalCost(decimal totalCost) => TotalCost = totalCost;
        public void SetAvgCost(decimal avgCost) => AvgCost = avgCost;
        public void SetAdjustmentCost(decimal adjustmentCost) => AdjustmentCost = adjustmentCost;
        public void SetLatestCost(decimal latestCost) => LatestCost = latestCost;

        public void UpdateStock(
            decimal qtyOnHand,
            decimal totalCost,
            decimal avgCost)
        {
            QtyOnHand = qtyOnHand;
            TotalCost = totalCost;
            AvgCost = avgCost;
        }

    }
}
