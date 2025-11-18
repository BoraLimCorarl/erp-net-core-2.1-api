using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.PayBills.Dto
{
    public class GetListPayBillInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
      
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid> Vendors { get; set; }
        public List<Guid?> BillIds { get; set; }
        public List<long?> Users { get; set; }
        public List<Guid> Accounts { get; set; }
        public List<TransactionStatus> Status { get; set; }
        public List<long?>Locations { get; set; }
        public List<long>VendorTypes { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "PaymentDate.Date";
            }
        }
    }
    public class GetListViewHistoryInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public Guid Id { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "JournalNo";
            }
        }
    }
}
