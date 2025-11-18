using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.ItemReceipts.ItemReceipt;
using static CorarlERP.Journals.Dto.JournalDetailOutput;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.ItemReceipts.Dto
{
    public class GetListItemReceiptInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid> Items { get; set; }
        public List<Guid?> Vendors { get; set; }
        public List<Guid?> Customers { get; set; }
        public List<Guid> Bills { get; set; }
        public List<Guid?> Accounts { get; set; }
        public List<long?> Users { get; set; }
        public List<Guid> OrderNo { get; set; }
        public List<Guid> JournalTransactionTypeIds { get; set; }
        // public List<ReceiveFrom> ReceiveFrom { get; set; }
        public List<TransactionStatus> Status { get; set; }  
        public List<JournalType> JournalTypes { get; set; }
        public List<long> Locations { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }
    public class GetItemReceiptInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
       
        public List<Guid> Vendors { get; set; }
        public List<long?> Locations { get; set; }       
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }

    public class GetNewItemReceiptInput : PagedSortedAndFilteredInputDto, IShouldNormalize
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
                Sorting = "Date.Date";
            }
        }
    }



    public class GetInventoryReportInput : PagedSortedAndFilteredInputDto//, IShouldNormalize
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid?> InventoryAccount { get; set; }
        public List<long> Location { get; set; }
        
        //public void Normalize()
        //{
        //    if (string.IsNullOrEmpty(Sorting))
        //    {
        //        Sorting = "JournalNo";
        //    }
        //}
    }
}
