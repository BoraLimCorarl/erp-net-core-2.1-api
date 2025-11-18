using CorarlERP.BatchNos;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Items.Dto;
using CorarlERP.Lots.Dto;
using CorarlERP.Taxes.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ItemIssueVendorCredits.Dto
{
    public class CreateOrUpdateItemIssueVendorCreditItemInput
    {
        public Guid? Id { get; set; }

        public Guid? VendorCreditItemId { get; set; }

        public Guid ItemId { get; set; }
        public ItemSummaryDetailOutput Item { get; set; }

        public string Description { get; set; }

        public decimal Qty { get; set; }

        //public long TaxId { get; set; }
        //public TaxSummaryOutput Tax { get; set; }

        public decimal UnitCost { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal Total { get; set; }

        public Guid InventoryAccountId { get; set; }
        public ChartAccountSummaryOutput InventoryAccount { get; set; }

        public Guid COGSAccountId { get; set; }
        public ChartAccountSummaryOutput COGSAccount { get; set; }
        public decimal OginalQtyFromPurchase { get; set; }

        public Guid? PurchaseAccountId { get; set; }
        public ChartAccountSummaryOutput PurchaseAccount { get; set; } 

        public long? FromLotId { get; set; }
        public LotSummaryOutput FromLotDetail {get;set;}

        public Guid? ItemReceiptPurhcaseItemId { get; set; }
        public DateTime CreationTime { get; set; }
        public bool UseBatchNo { get; set; }
        public List<BatchNoItemOutput> ItemBatchNos { get; set; }
        public bool TrackSerial { get; set; }
        public bool TrackExpiration { get; set; }
    }
}
