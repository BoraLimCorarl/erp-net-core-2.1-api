using CorarlERP.Items.Dto;
using CorarlERP.Taxes.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.KitchenOrders.Dto
{
   public class CreateOrUpdateKitchenOrderItemInput
    {
        public Guid? Id { get; set; }
        public Guid ItemId { get; set; }
        public ItemSummaryOutput Item { get; set; }
        public long TaxId { get; set; }
        public TaxDetailOutput Tax { get; set; }
        public string Description { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitCost { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal Total { get; set; }
        public decimal TaxRate { get; set; }   
        public decimal MultiCurrencyTotal { get; set; }
        public decimal MultiCurrencyUnitCost { get; set; }
        public Guid BOMId { get; set; }
        public string BomName { get; set; }
        public List<CreateKitchenOrderItemAndBOMItemInput> KitchenOrderItemAndBOMItem { get; set; }
    }

    public class KitchenOrderItemImportOutput {

        public Guid BOMId { get; set; }   
        public string Group { get; set; }
        public Guid Id { get; set; }

    }

}
