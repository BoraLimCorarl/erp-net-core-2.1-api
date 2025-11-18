using CorarlERP.Items.Dto;
using CorarlERP.Lots.Dto;
using CorarlERP.Taxes.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.KitchenOrders.Dto
{
    public class CreateKitchenOrderItemAndBOMItemInput
    {

        public Guid? Id { get; set; }
        public decimal Qty { get;  set; }
        public decimal TotalQty { get;  set; }
        public Guid ItemId { get; set; }
        public ItemSummaryOutput Item { get; set; }
        public GetBomItemDetail BomItem { get;  set; }
        public Guid BomItemId { get;  set; }
        public Guid? KitchenOrderItemId { get; set; }
        public LotSummaryOutput Lot { get; set; }
        public long? LotId { get; set; }
        public long TaxId { get; set; }
        public TaxDetailOutput Tax { get; set; }
        public decimal TaxRate { get; set; }

    }
}
