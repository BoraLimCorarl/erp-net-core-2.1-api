using Abp.Runtime.Validation;
using CorarlERP.Dto;
using CorarlERP.InventoryCalculationSchedules;
using CorarlERP.TransactionTypes;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Invoices.Dto
{
    public class GetListInventoryCalculationScheduleInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        //public DateTime? FromDate { get; set; }
        //public DateTime ToDate { get; set; }     
        public bool? IsActive { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "NextExecution";
            }
        }
    }
    
}
