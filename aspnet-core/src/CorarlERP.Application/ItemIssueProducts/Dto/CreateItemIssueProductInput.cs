using CorarlERP.Addresses;
using CorarlERP.Productions.Dto;
using System;
using System.Collections.Generic;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ItemIssueProducts.Dto
{
    public class CreateItemIssueProductInput
    {
        public ReceiveFrom ReceiveFrom { get; set; }
        public Guid ClearanceAccountId { get; set; }
        public Guid? ProductionId { get; set; }
        public long? ProductionProcessId { get; set; }
        public long LocationId { get; set; }

        public bool SameAsShippingAddress { get; set; }

        public CAddress BillingAddress { get; set; }

        public CAddress ShippingAddress { get; set; }

        public decimal Total { get; set; }

        public string Reference { get; set; }

        public string IssueNo { get; set; }

        public string Memo { get; set; }

        public DateTime Date { get; set; }

        public long CurrencyId { get; set; }

        public long? ClassId { get; set; }

        public TransactionStatus Status { get; set; }

        public List<CreateOrUpdateRawMaterialItemsInput> Items { get; set; }

        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }
    }
}
