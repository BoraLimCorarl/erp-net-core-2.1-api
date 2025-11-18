using Abp.AutoMapper;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Items;
using CorarlERP.Items.Dto;
using CorarlERP.Taxes.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.PurchaseOrders.Dto
{
    [AutoMapFrom(typeof(PurchaseOrderItem))]
   public class PurchaseOrderItemDetailOut
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public ItemSummaryOutput Item { get; set; }
        public TaxDetailOutput Tax { get; set; }
        public long TaxId { get; set; }
        public string Description { get; set; }
        public decimal Unit { get; set; }
        public decimal TotalReceiptQty { get; set; }
        public decimal TotalReceiptBillQty { get; set; }
        public decimal UnitCost { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal TaxRate { get; set; }
        public decimal Total { get; set; }
        public decimal Remain { get; set; }
        public decimal TotalBillQty { get; set; }
        public decimal MultiCurrencyTotal { get; set; }
        public decimal MultiCurrencyUnitCost { get; set; }
    }

    [AutoMapFrom(typeof(PurchaseOrderItem))]
    public class PurchaseOrderItemSummaryOut
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public ItemSummaryDetailOutput Item { get; set; }       
        public decimal Unit { get; set; }
        public decimal Total { get; set; }
        public decimal UnitCost { get; set; }
        public decimal MultiCurrencyTotal { get; set; }
        public decimal MulitCurrencyUnitCost { get; set; }
        public decimal DiscountRate { get; set; }       
        public TaxDetailOutput Tax { get; set; }
        public decimal Remain { get; set; }        
        public long TaxId { get; set; }
        public string Description { get; set; }
        public bool UseBatchNo { get; set; }
        public bool AutoBatchNo { get; set; }
        public long? LotId { get; set; }
        public string LotName { get; set; }
        public bool TrackSerial { get; set; }
        public bool TrackExpiration { get; set; }
    }
}
