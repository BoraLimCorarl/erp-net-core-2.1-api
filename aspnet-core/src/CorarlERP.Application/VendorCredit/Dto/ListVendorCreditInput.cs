using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.VendorCredit.Dto
{
    public class ListVendorCreditInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        //public DateTime FromDate { get; set; }
        //public DateTime ToDate { get; set; }
        public List<long?> Locations { get; set; }
        public List<Guid> Vendors { get; set; }
        //public List<PaidStatuse> PaidStatus { get; set; }
        //public List<TransactionStatus> Status { get; set; }
        //public List<JournalType> Type { get; set; }
        public  List<Guid> PayBillId { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "JournalNo";
            }
        }
    }


    public class GetVendorCreditInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid> Vendors { get; set; }
        public List<long?> Locations { get; set; }
        public List<long?> Lots { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid?> Items { get; set; }
        public void Normalize()
        {
            Sorting = "Date.Date";
        }
    }

    public class GetVendorCreditInputForItem
    {
        public Guid Id { get; set; }
        public DateTime? Date { get; set; }
    }

    public class GetVendorCreditFromIssueCreditInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid> Vendors { get; set; }
        public List<long?> Locations { get; set; }
        public List<long?> Lots { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid?> Items { get; set; }
        public void Normalize()
        {
            Sorting = "Date.Date";
        }
    }


}
