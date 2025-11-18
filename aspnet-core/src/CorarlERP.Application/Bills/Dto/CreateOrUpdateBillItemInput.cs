using CorarlERP.BatchNos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Bills.Dto
{
   public class CreateOrUpdateBillItemInput
    {
        public Guid? Id { get; set; }        
        public decimal OrginalQtyFromPurchase { get; set; }
        public string ItemName { get; set; }
        public Guid? OrderItemId { get; set; }
        public Guid? PurchaseOrderId { get; set; }
        public Guid? ItemReceiptId { get; set; }

        public Guid? ItemId { get; set; }
        public long? ItemTypeId { get; set; }
       

        public string Description { get; set; }

        public long TaxId { get; set; }

        public decimal Qty { get; set; }

        public decimal UnitCost { get; set; }

        public decimal DiscountRate { get; set; }

        public decimal Total { get; set; }

        public Guid InventoryAccountId { get; set; }
      
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

    }
}
