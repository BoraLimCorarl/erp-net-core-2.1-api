using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.BankTransactions.Dto
{
  public  class GetListBankTransactionInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid>Vendors { get; set; }
        public List<Guid>Customers { get; set; }
        public List<Guid>Accounts { get; set; }
        public List<long?> Users { get; set; }
        public List<TransactionStatus> Status { get; set; }
        public List<long> Locations { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }
}
