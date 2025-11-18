using Abp.Runtime.Validation;
using CorarlERP.Dto;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.Bills.Dto
{
   public class GetListBillInput : PagedSortedAndFilteredInputDto, IShouldNormalize
   {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid?> Items { get; set; }
        public List<Guid> Vendors { get; set; }
        public List<PaidStatuse> PaidStatus { get; set; }
        public List<long?> Users { get; set; }
        public List<Guid> Accounts { get; set; }

        // public List<ReceiveFrom> ReceiveFrom { get; set; }
        public List<TransactionStatus> Status { get; set; }
        public List<JournalType> Type { get; set; }
        public List<long> Locations { get; set; }
        public List<long> VendorTypes { get; set; }
        public List<long> ClassIds { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }

    public class GetListBillForPaybillInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid> Vendors { get; set; }
        public List<Guid> BillNo { get; set; }
        public List<long> Locations { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "JournalNo";
            }
        }
    }

    public class GetBillListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid> Vendors { get; set; }
        public List<long?> Locations { get; set; }
        public void Normalize()
        {

            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "BillNo";
            }

        }
    }
    public class GetListDeleteInput { 
     
        public List<Guid> Ids { get; set; }
    }
}
