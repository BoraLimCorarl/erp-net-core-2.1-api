using Abp.Runtime.Validation;
using CorarlERP.Dto;
using CorarlERP.TransactionTypes;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Invoices.Dto
{
    public class GetListInvoiceInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid?> Items { get; set; }
        public List<Guid> Customers { get; set; }
        public List<Guid?> ItemIssues { get; set; }
        public List<long?> Users { get; set; }
        public List<Guid> Accounts { get; set; }
        public List<PaidStatuse> PaidStatus { get; set; }
        public List<TransactionStatus> Status { get; set; }
        public List<JournalType> Type { get; set; }
        public List<long?> TransactionSaleType { get; set; }
        public List<long>Locations { get; set; }
        public List<long>CustomerTypes { get; set; }
        public List<long> ClassIds { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }


    public class GetInvoiceListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
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

    public class GetListInvoiceForPaybillInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid> Customers { get; set; }
        public List<Guid> InvoiceNo { get; set; }
        public List<long> Locations { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "JournalNo";
            }
        }
    }
}
