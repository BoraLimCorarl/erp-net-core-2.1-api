using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.BankTransfers.Dto
{
    public class GetListBankTrasferInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<long?> Users { get; set; }
        public List<long?>Locations { get; set; }
        public void Normalize()
        {          
                if (string.IsNullOrEmpty(Sorting))
                {
                    Sorting = "BankTransferNo";
                }
        }
    }
}
