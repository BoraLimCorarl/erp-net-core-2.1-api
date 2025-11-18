using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ItemIssues.Dto
{
    public class GetListItemIssueInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid> Items { get; set; }
        public List<Guid?> Customers { get; set; }
        public List<Guid?> Vendors { get; set; }
        public List<Guid> Invoices { get; set; }
        public List<Guid> SaleOrderNo { get; set; }   
        public List<TransactionStatus> Status { get; set; }
        public List<long?> Users { get; set; }
        public List<Guid?> Accounts { get; set; }
        public List<long?> TransactionSaleType { get; set; } 
        public List<JournalType> JournalTypes { get; set; }
        public List<long> Locations { get; set; }
        public List<Guid> JournalTransactionTypeIds { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }

    public class GetItemIssueInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid> Customers { get; set; }
        public List<long?> Locations { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }


    public class GetItemIssueVendorCreditInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid> Vendors { get; set; }
        public List<long?> Locations { get; set; }
        public List<long?> Lots { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid> Items { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "ItemIssueNo";
            }
        }
    }



    public class GetInventoryReportInput : PagedSortedAndFilteredInputDto//, IShouldNormalize
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid?> InventoryAccount { get; set; }
        public List<long> Location { get; set; }     
    }


    public class GetForCustomerCreditItemIssueInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid> Customers { get; set; }
        public List<long?> Locations { get; set; }
        public List<long?> Lots { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid?> Items { get; set; }
        public FilterType FilterType { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }

        }
    }


    public class GetForCustomerCreditItemInvoiceInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid> Customers { get; set; }
        public List<long?> Locations { get; set; }
        public bool? IsActive { get; set; }
        public List<long?> PropertyValueIds { get; set; }
        public List<long?> CustomerTypes { get; set; }      
        public FilterType FilterType { get; set; }


        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "invoiceNo";
            }

        }
    }
}
