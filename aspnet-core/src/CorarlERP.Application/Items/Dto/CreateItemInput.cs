using CorarlERP.SubItems.Dto;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.Lots.Dto;

namespace CorarlERP.Items.Dto
{
    public class CreateItemInput
    {
       
        public string ItemCode { get;  set; }
        [Required]
        public string ItemName { get;  set; }

        public decimal? PurchaseCost { get; set; }

        public decimal? SalePrice { get; set; }

        public decimal? ReorderPoint { get; set; }

        public bool TrackSerial { get; set; }

        public string Description { get; set; }
        
        [Required]
        public long ItemTypeId { get; set; }

        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public long? SaleCurrenyId { get; set; }
      
        public long? PurchaseCurrencyId { get; set; }
           
        public Guid? SaleAccountId { get; set; }
    
        public Guid? PurchaseAccountId { get; set; }
      
        public Guid? InventoryAccountId { get; set; }
       
        public long? PurchaseTaxId { get; set; }
       
        public long? SaleTaxId { get; set; }
        public Member Member { get; set; }

        public List<CreateOrItemPropertyInput> Properties { get; set; }

       // public List <CreateSubItemInput> SubItems { get; set; }
        public List<GroupItems> UserGroups { get; set; }

        public bool ShowSubItems { get; set; }

        public List<ItemLotDto> DefaultLots { get; set; }

        public Guid? ImageId { get; set; }
        public string Barcode { get; set; }

        public bool UseBatchNo { get; set; }
        public bool AutoBatchNo { get; set; }
        public long? BatchNoFormulaId { get; set; }
        public bool TrackExpiration { get; set; }
        public bool BarcodeSameAsItemCode { get; set; }        

    }

    public class createImport
    {
     
        public string ItemCode { get; set; }
     
        public string ItemName { get; set; }

        public Guid? InventoryAccountId { get; set; }

        public decimal? PurchaseCost { get; set; }

        public decimal? SalePrice { get; set; }

        public Guid? SaleAccountId { get; set; }

        public Guid? PurchaseAccountId { get; set; }

        public long? PurchaseTaxId { get; set; }

        public long? SaleTaxId { get; set; }

        public long ItemTypeId { get; set; }

        public string Description { get; set; }
        public  bool BarcodeSameAsItemCode { get; set; }



    } 

}
