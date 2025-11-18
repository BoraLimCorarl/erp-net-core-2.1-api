using Abp.AutoMapper;
using CorarlERP.BatchNos;
using CorarlERP.Bills;
using CorarlERP.Bills.Dto;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Items;
using CorarlERP.Items.Dto;
using CorarlERP.Journals;
using CorarlERP.Journals.Dto;
using CorarlERP.Lots.Dto;
using CorarlERP.PurchaseOrders;
using CorarlERP.PurchaseOrders.Dto;
using CorarlERP.Taxes;
using CorarlERP.Taxes.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemReceipts.Dto
{
    [AutoMapFrom(typeof(ItemReceiptItem))]
    public class ItemReceiptItemDetailOutput
    {
        public DateTime CreationTime { get; set; }
        public Guid Id { get; set; }

        public string OrderNumber { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public Guid?  OrderItemId { get; set; }
        public PurchaseOrderItemSummaryOut PurchaseOrderItem { get; set; }

        public Guid? PurchaseOrderId { get; set; }

        public Guid ItemId { get;  set; }
        public ItemSummaryOutput Item { get; set; }

        public string Description { get; set; }
    

        public decimal Qty { get; set; }

        public decimal UnitCost { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal Total { get; set; }

        public Guid InventoryAccountId { get; set; }
        public ChartAccountSummaryOutput InventoryAccount { get; set; }

        public Guid? BillItemId { get; set; }
        public BillSummaryOutput BillItem { get; set; }
       
        public LotSummaryOutput ToLotDetail { get; set; }
        public long? ToLotId { get; set; }
        public long? MultiCurrencyId { get; set; }

        public bool UseBatchNo { get; set; }
        public bool AutoBatchNo { get; set; }
        public List<BatchNoItemOutput> ItemBatchNos { get; set; }
        public bool TrackSerial { get; set; }
        public bool TrackExpiration { get; set; }
    }

    [AutoMapFrom(typeof(ItemReceiptItem))]
    public class ItemReceiptItemSummaryOutput
    {

        public Guid? PurchaseOrderItemId { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        public string OrderNumber { get; set; }

        public Guid Id { get; set; }
     
        public Guid ItemId { get; set; }
        public ItemSummaryOutput Item { get; set; }

        public string Description { get; set; }

        //public long TaxId { get; set; }
        //public TaxSummaryOutput Tax { get; set; }

        public decimal Qty { get; set; }

        public decimal UnitCost { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal Total { get; set; }

        public Guid InventoryAccountId { get; set; }
        public ChartAccountSummaryOutput InventoryAccount { get; set; }

        public LotSummaryOutput ToLotDetail { get; set; }
        public long? ToLotId { get; set; }
        public bool UseBatchNo { get; set; }
        public bool AutoBatchNo { get; set; }
        public List<BatchNoItemOutput> ItemBatchNos { get; set; }
        public bool TrackSerial { get; set; }
        public bool TrackExpiration { get; set; }
    }
}
