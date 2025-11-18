using Abp.Runtime.Validation;
using Castle.MicroKernel.Registration;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Items.Dto
{
    public class GetItemListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool UsePagination { get; set; }
        public bool? IsActive { get; set; }
        public List<long> ItemTypes { get; set; }
        public List<Guid?> PurchaseAccount { get; set; }
        public List<Guid?> SaleAccount { get; set; }
        public List<Guid?> InventoryAccount { get; set; }
        //public List<long?> Properties { get; set; }
        //public Dictionary<long, List<long?>> PropertyDics { get; set; }
        public List<GetListPropertyFilter> PropertyDics { get; set; }

        public bool IsPurchase { get; set; }
        public bool IsHasAccountId { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "ItemName";
            }

        }
    }

    public class GetItemListInputFind : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public DateTime Date { get; set; }
        public bool IsFilterAverageCost { get; set; }

        public bool? IsActive { get; set; }
        public List<long> ItemTypes { get; set; }
        public List<Guid?> PurchaseAccount { get; set; }
        public List<Guid?> SaleAccount { get; set; }
        public List<Guid?> InventoryAccount { get; set; }
        public List<long?> Locations { get; set; }
        public List<GetListPropertyFilter> PropertyDics { get; set; }
        public bool IsHasPurchaseAccountId { get; set; }
        public bool IsHasAccountId { get; set; }
        public bool IsHasBalance { get; set; }
        public bool IsHasSaleAccountId { get; set; }
        public bool IncludeService { get; set; }
        public List<Guid?> UpdateItemIssueIds { get; set; }
        public bool IsProduction { get; set; }

        public bool IsTransfer { get; set; }

        public bool IsFilterService { get; set; }

        public List<Guid> SelectedItemIds { get; set; }
        public bool IsPurchasePrice { get; set; }
        public Guid? VendorId { get; set; }
        public long? CurrencyId { get; set; }

        public FilterType FilterType { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "ItemCode";
            }
        }
       

    }
    public class GetPrintBarcodeInput
    {
        public Guid Id { get; set; }
        public int Qty { get; set; }
    }
    public class GetListPrintBarcodeInput
    {
        public List<GetPrintBarcodeInput> Items { get; set; }
    }
    public class GetPrintBarcodeOutput
    {
        public Guid Id { get; set; }
        public int Qty { get; set; }
        public string Barcode { get; set; }
    }
    public class GetItemListInputPOSFind : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public DateTime Date { get; set; }
        public bool IsFilterAverageCost { get; set; }

        public bool? IsActive { get; set; }
        public List<long> ItemTypes { get; set; }
        public List<Guid?> PurchaseAccount { get; set; }
        public List<Guid?> SaleAccount { get; set; }
        public List<Guid?> InventoryAccount { get; set; }
        public List<long?> Locations { get; set; }
        public List<long?> PropertyValueIds { get; set; }
        public List<long?> CustomerTypes { get; set; }
        public bool IsHasPurchaseAccountId { get; set; }
        public bool IsHasAccountId { get; set; }
        public bool IsHasBalance { get; set; }
        public bool IsHasSaleAccountId { get; set; }
        public bool IncludeService { get; set; }
        public List<Guid?> UpdateItemIssueIds { get; set; }
        public bool IsProduction { get; set; }

        public bool IsTransfer { get; set; }

        public bool IsFilterService { get; set; }
        public FilterType FilterType { get; set; }

        public void Normalize()
        {

            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "ItemCode";
            }
            this.FilterType = FilterType.Contain;

        }
    }

        public class ItemIdsListInput
    {
        public DateTime Date { get; set; }
        public bool IsFilterAverageCost { get; set; }
        
        public List<long?> Locations { get; set; }

        public List<Guid> ItemIds { get; set; }
        public List<Guid?> UpdateItemIssueIds { get; set; }
        public bool IsProduction { get; set; }

        public bool IsTransfer { get; set; }
        
        
    }

    public class GetSubitemInput 
    {
        public Guid PernetId { get; set; }
        public DateTime Date { get; set; }
        public bool GetStockBalance { get; set; }
        public long LocationId { get; set; }
        public List<Guid?> ExceptIds { get; set; }        
        public decimal Qty { get; set; }
    }
}
