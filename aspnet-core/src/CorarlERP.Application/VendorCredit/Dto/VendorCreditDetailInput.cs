using Abp.AutoMapper;
using CorarlERP.BatchNos;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.ItemIssues.Dto;
using CorarlERP.Items.Dto;
using CorarlERP.Lots.Dto;
using CorarlERP.Taxes;
using CorarlERP.Taxes.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.VendorCredit.Dto
{
    [AutoMapFrom(typeof(VendorCreditDetail))]
    public class VendorCreditDetailInput
    {
        public Guid? Id { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? ItemId { get; set; }
        public ItemSummaryDetailOutput Item { get; set; }

        public Guid? ItemReceiptItemPurchaseId { get; set; }
        public Guid? ItemIssueItemVendorCreditId { get; set; }

        public decimal? PurchaseCost { get; set; }

        public string Description { get; set; }

        public long TaxId { get; set; }
        public TaxSummaryOutput Tax { get; set; }

        public decimal Qty { get; set; }
        public decimal OginalQtyFromPurchase {get;set;}
        public decimal UnitCost { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal Total { get; set; }

        public decimal ItemIssueVendorCreditUnitCost { get; set; }
        public decimal ItemIssueVendorCreditTotal { get; set; }

        public Guid AccountId { get; set; }
        public ChartAccountSummaryOutput Account { get; set; }

        public long? FromLotId { get; set; }
        public LotSummaryOutput LotDetail { get; set; }


        public decimal MultiCurrencyTotal { get; set; }
        public decimal MultiCurrencyUnitCost { get; set; }
        public bool UseBatchNo { get; set; }
        public List<BatchNoItemOutput> ItemBatchNos { get; set; }
        public bool TrackSerial { get; set; }
        public bool TrackExpiration { get; set; }
       
    }
}
