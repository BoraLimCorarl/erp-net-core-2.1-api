using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.PayBills.Dto
{   
    public class GetListVendorBalanceInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid> Vendors { get; set; }
        public List<long> VendorTypes { get; set; }
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
