using CorarlERP.Dto;
using CorarlERP.Locations.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.TransferOrders.Dto
{
    public class TransferOrderGetListOutput
    {
        public Guid Id { get; set; }
        public TransactionStatus StatusCode { get; set; }

        public int CountItem { get; set; }
        public TransferStatus ReceiveStatus { get; set; }
        public string TransferNo { get; set; }
        public DateTime TransferDate { get; set; }
        public bool CanDrafOrVoid { get; set; }
        public LocationSummaryOutput FromLocation { get; set; }
        public LocationSummaryOutput ToLocation { get; set; }
        public UserDto User { get; set; }
        public string Reference { get; set; }
        public string LastModifiedUserName { get; set; }

    }
}
