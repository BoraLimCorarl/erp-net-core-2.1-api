using Abp.AutoMapper;
using CorarlERP.BatchNos;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Items.Dto;
using CorarlERP.Lots.Dto;
using CorarlERP.PurchaseOrders.Dto;
using CorarlERP.Taxes.Dto;
using System;
using System.Collections.Generic;

namespace CorarlERP.Bills.Dto
{
    [AutoMapFrom(typeof(BillItem))]
    public class BillItemDetailOutput
    {
        public decimal OriginalQtyFromPurchase { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }      
        public Guid? OrderItemId { get; set; }
        public Guid? PurchaseOrderId { get; set; }//Id header of PO (Primary)
        public PurchaseOrderItemSummaryOut PurchaseOrderItem { get; set; }
        public bool IsItemReceipt { get; set; }
        public Guid? ItemId { get; set; }
        public ItemSummaryOutput Item { get; set; }

        public string Description { get; set; }

        public long TaxId { get; set; }
        public TaxSummaryOutput Tax { get; set; }

        public decimal Qty { get; set; }

        public decimal UnitCost { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal Total { get; set; }

        public Guid InventoryAccountId { get; set; }
        public ChartAccountSummaryOutput InventoryAccount { get; set; }

        public Guid? ItemReceiptId { get; set; }
        //public ItemReceiptSummaryOutput ItemReceipt { get; set; }

        public LotSummaryOutput LotDetail { get; set; }
        public long? LotId { get; set; }

        public decimal MultiCurrencyTotal { get; set; }
        public decimal MultiCurrencyUnitCost { get; set; }
        public Guid? Key { get; set; }
        public Guid? ParentId { get; set; }
        public bool DisplayInventoryAccount { get; set; }
        public bool Display { get; set; }
     
        public bool UseBatchNo { get; set; }
        public bool AutoBatchNo { get; set; }
        public List<BatchNoItemOutput> ItemBatchNos { get; set; }
        public bool TrackSerial { get; set; }
        public bool TrackExpiration { get; set; }
    }


    [AutoMapFrom(typeof(BillItem))]
    public class BillItemSummaryOutput
    {
        public Guid? PurchaseOrderId { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid Id { get; set; }
        public Guid? OrderItemId { get; set; }
        public Guid? ItemId { get; set; }
        public ItemSummaryDetailOutput Item { get; set; }

        public LotSummaryOutput LotDetail { get; set; }
        public long? LotId { get; set; }

        public string Description { get; set; }

        public long TaxId { get; set; }
        public TaxSummaryOutput Tax { get; set; }

        public decimal Qty { get; set; }

        public decimal UnitCost { get; set; }

        public decimal Total { get; set; }

        public Guid? ItemReceiptItemId { get; set; }

        public Guid InventoryAccountId { get; set; }
        public ChartAccountSummaryOutput InventoryAccount { get; set; }

        public decimal DiscountRate { get; set; }
        public Guid? Key { get; set; }
        public Guid? ParentId { get; set; }
        public bool DisplayInventoryAccount { get; set; }
        public bool Display { get; set; }
        public bool UseBatchNo { get; set; }
        public bool AutoBatchNo { get; set; }
        public List<BatchNoItemOutput> ItemBatchNos { get; set; }
        public bool TrackSerial { get; set; }
        public bool TrackExpiration { get; set;}
    }
}
