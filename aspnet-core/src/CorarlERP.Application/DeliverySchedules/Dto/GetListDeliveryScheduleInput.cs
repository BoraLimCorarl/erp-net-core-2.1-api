using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.DeliverySchedules.Dto
{
    public class GetListDeliveryScheduleInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<long> CustomerTypes { get; set; }
        public List<Guid> Customers { get; set; }
        public List<Guid> Items { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<TransactionStatus> Status { get; set; }
        public List<long?> Users { get; set; }
        public List<long> Locations { get; set; }
        public List<DeliveryStatus> DeliveryStatuses { get; set; }
        public void Normalize()
        {

            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "InitialDeliveryDate";
            }

        }
    }
}
