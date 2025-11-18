using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.TransferOrders.Dto
{
    public class GetTransferOrderListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid?> Items { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsForItemIssue { get; set; }
        public bool IsForItemReceipt { get; set; }
        
        public List<long?> Users { get; set; }
        public List<TransferStatus?> DeliveryStatus { get; set; }
        public List<long?> FromLocations { get; set; }
        public List<long?> ToLocations { get; set; }
        public List<long>Locactions { get; set; }
        public void Normalize()
        {

            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "TransferDate.Date, TransferNo";
            }

        }
    }
}
