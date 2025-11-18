using Abp.Runtime.Validation;
using CorarlERP.Dto;
using CorarlERP.TransactionTypes;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.POS.Dto
{    
    public class GetUnpaidPOSInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<long?> Users { get; set; }
        public List<long> Locations { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date";
            }
        }
    }

}
