using Abp.AutoMapper;
using CorarlERP.Boms;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Lots.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Items.Dto
{
    [AutoMapFrom(typeof(BomItem))]
    public class GetBomItemOutput
    {
        public string ItemName { get; set; }
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public Guid BOMId { get; set; }
        public decimal Qty { get; set; }
        public Guid? Id { get; set; }
        public Guid? SaleAccountId { get; set; }     
        public Guid? InventoryAccountId { get; set; }
        public Guid? PurchaseAccountId { get; set; }  
        public string LotName { get; set; }
        public long? LotId { get; set; }     
        public long? PurchaseTaxId { get; set; }
        public long? PurchaseCurrencyId { get; set; }
        public long? SaleTaxId { get; set; }
        public long? SaleCurrenyId { get; set; }
        public long? LocationId { get; set; }

    }

    public class BomItemInput { 
    
       public Guid BomId { get; set; }
       public List<long> Locations { get; set; }
             
    }


    [AutoMapFrom(typeof(BomItem))]
    public class GetBomItemDetail
    {
        public string ItemName { get; set; }
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public Guid BOMId { get; set; }
        public decimal Qty { get; set; }
        public Guid? Id { get; set; }
    }

    public class GetBomItemExcel {
        public string ItemName { get; set; }
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public Guid BOMId { get; set; }
        public decimal Qty { get; set; }
        public Guid? Id { get; set; }
        public string BomName { get; set; }
        public string MainOutputItemCode { get; set; }

    }

}
