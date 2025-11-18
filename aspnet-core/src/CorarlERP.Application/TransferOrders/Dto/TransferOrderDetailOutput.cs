using Abp.AutoMapper;
using CorarlERP.Classes.Dto;
using CorarlERP.Locations.Dto;
using System;
using System.Collections.Generic;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.TransferOrders.Dto
{
    [AutoMapFrom(typeof(TransferOrder))]
    public class TransferOrderDetailOutput
    {
        public Guid Id { get; set; }
        public string TransferNo { get; set; }
        public DateTime TransferDate { get; set; }
        public string Reference { get; set; }
        public TransactionStatus Status { get; set; }
        public TransferStatus ShipedStatus { get; set; }

        public long TransferToLocationId { get; set; }
        public LocationSummaryOutput TransferToLocation { get; set; }
        public long TransferFromLocationId { get; set; }
        public LocationSummaryOutput TransferFromLocation { get; set; }


        public long TransferToClassId { get; set; }
        public ClassSummaryOutput TransferToClass { get; set; }
        public long TransferFromClassId { get; set; }
        public ClassSummaryOutput TransferFromClass { get; set; }
        
        public string Memo { get; set; }
        public List<CreateOrUpdateTransferOrderItemInput> TransferOrderItems { get; set; }
        public bool ConvertToIssueAndReceiptTransfer { get; set; }
        public DateTime? ItemReceiptTransferDate { get; set; }
        public DateTime? ItemIssueTransferDate { get; set; }
        public Guid? ItemIssueId { get; set; }
        public Guid? ItemReceiptId { get; set; }
    }
}
