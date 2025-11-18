using CorarlERP.Addresses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.TransferOrders.Dto
{
    public class CreateTransferOrderInput
    {
        public TransactionStatus Status { get; set; }
        
        public string TransferNo{ get; set; }
        public DateTime TransferDate { get; set; }
        public string Reference { get; set; }
        public long TransferFromLocationId { get; set; }
        public long TransferToLocationId { get; set; }
        public long TransferFromClassId { get; set; }
        public long TransferToClassId { get; set; }
        public string Memo { get; set; }
        public List<CreateOrUpdateTransferOrderItemInput> TransferOrderItems { get; set; }
        public bool ConvertToIssueAndReceiptTransfer { get; set; }
        public DateTime? ItemReceiptTransferDate { get; set; }
        public DateTime? ItemIssueTransferDate { get; set; }
        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }
    }
}
