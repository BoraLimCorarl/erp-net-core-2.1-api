using CorarlERP.BatchNos;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.DeliverySchedules.Dto;
using CorarlERP.Items.Dto;
using CorarlERP.Lots.Dto;
using CorarlERP.SaleOrders.Dto;
using CorarlERP.Taxes.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Invoices.Dto
{
    public class CreateOrUpdateInvoiceItemInput
    {
        public DateTime CreationTime { get; set; }
        public Guid? Id { get; set; }
        public Guid? OrderItemId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderReference { get; set; }
        public Guid? SaleOrderId { get; set; }
        public SaleOrderSummaryOutput SaleOrder { get; set; }
        public SaleOrderItemSummaryOut SaleOrderItem { get; set; }
        public Guid? ItemIssueId { get; set; }

        public Guid? ItemId { get; set; }
        public ItemSummaryDetailOutput Item { get; set; }
        public string Description { get; set; }
        public long TaxId { get; set; }
        public TaxSummaryOutput Tax { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitCost { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal Total { get; set; }
        public Guid AccountId { get; set; }
        public ChartAccountSummaryOutput Account { get; set; }
        public bool IsItemIssue { get; set; }

        public long? LotId { get; set; }
        public LotSummaryOutput LotDetail { get; set; }

        public decimal MultiCurrencyTotal { get; set; }
        public decimal MultiCurrencyUnitCost { get; set; }

        public decimal UnitCostItemIssue { get; set; }
        public decimal TotalItemIssue { get; set; }

        public decimal OrginalQtyFromSaleOrder { get; set; }
        public string ItemName { get; set; }
        public Guid? Key { get; set; }
        public Guid? ParentId { get; set; }
        public bool DisplayInventoryAccount { get; set; }
        public bool Display { get; set; }
        public bool UseBatchNo { get; set; }
        public List<BatchNoItemOutput> ItemBatchNos { get; set; }
        public bool TrackSerial { get; set; }
        public bool TrackExpiration { get; set; }

        public Guid? DeliveryScheduleItemId { get; set; }
        public string DeliveryNo { get; set; }
        public string DeliveryReference { get; set; }
        public Guid? DeliveryId { get; set; }
        public DeliveryScheduleSummaryOutput DeliverySchedule { get; set; }
        public DeliveryItemSummaryOut DeliveryScheduleItem { get; set; }
        public decimal OrginalQtyFromDeliverySchedule { get; set; }

    }
}
