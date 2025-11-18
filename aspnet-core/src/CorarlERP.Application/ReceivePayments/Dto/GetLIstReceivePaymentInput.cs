using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ReceivePayments.Dto
{   
    public class GetLIstReceivePaymentInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid> Customers { get; set; }
        public List<long> CustomerTypes { get; set; }
        public List<Guid?> InvoiceId { get; set; }
        public List<Guid> Accounts { get; set; }
        public List<TransactionStatus> Status { get; set; }
        public List<long>Locations { get; set; }
        public List<long?> Users { get; set; }
        public bool UsePagination { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "PaymentDate.Date";
            }
        }
    }
    public class GetListHistoryInput : PagedSortedAndFilteredInputDto, IShouldNormalize
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
