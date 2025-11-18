using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ReceivePayments.Dto
{   
    public class GetListCustomerBalanceInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid> Customers { get; set; }
        public List<long> CustomerTypes { get; set; }
        public List<Guid> Accounts { get; set; }
        public List<long> AccountTypes { get; set; }
        public List<long>Locations { get; set; }
        public CurrencyFilter CurrencyFilter { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Balance";
            }
        }
    }
  
}
