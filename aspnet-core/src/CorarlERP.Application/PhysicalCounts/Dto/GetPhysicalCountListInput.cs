using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.PhysicalCounts.Dto
{
    public class GetPhysicalCountListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid?> Items { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public List<TransactionStatus?> Status { get; set; }
        public List<long?> Locations { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "PhysicalCountNo";
            }

        }
    }
}
