using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Items.Dto;
using CorarlERP.Taxes.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.SaleOrders.Dto
{
    public class CreateOrUpdateSaleOrderItemInput
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
        public decimal Remain { get; set; }
        public decimal MultiCurrencyTotal { get; set; }
        public decimal MultiCurrencyUnitCost { get; set; }

    }
}
