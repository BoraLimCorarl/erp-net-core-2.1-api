using Abp.Runtime.Validation;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Dto;
using CorarlERP.Lots.Dto;
using CorarlERP.Taxes.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Items.Dto
{
    public  class GetBomOutput
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public bool IsDefault { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public decimal Qty { get; set; }
        public List<GetBomItemOutput> BOMItems { get; set; }
        
    }

    public class GetBomOutputSummary
    {
        public Guid Id { get; set; } 
        public string Name { get; set; }
        public Guid ItemId { get; set; }
        public bool IsDefault { get; set; }
     

    }
    public class GetListItemFromBOMOutput
    {
        public Guid Id { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public Guid? BomId { get; set; }
        public string BomName { get; set; }
        public Guid? ImageId { get; set; }
        public string Barcode { get; set; }
        public long? SaleTaxId { get; set; }
        //public List<GetBomOutput> Boms { get; set; }

    }

    public class GetListItemForBOMOutput
    {
        public Guid Id { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public Guid? ImageId { get; set; }
        public string Barcode { get; set; }


    }

    public class GetListItemForBOMInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public FilterType FilterType { get; set; }
        public List<GetListPropertyFilter> PropertyDics { get; set; }
        public List<Guid> SelectedItemIds { get; set; }
        public List<long> Locations { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "ItemName";
            }

        }
    }

}
