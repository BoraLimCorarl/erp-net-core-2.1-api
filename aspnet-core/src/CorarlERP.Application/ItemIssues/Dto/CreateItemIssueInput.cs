using CorarlERP.Addresses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ItemIssues.Dto
{
   public class CreateItemIssueInput
    {
        public ReceiveFrom ReceiveFrom { get; set; }

        public Guid? InvoiceId { get; set; }

        public Guid CustomerId { get; set; }

        public long LocationId { get; set; }

        public bool SameAsShippingAddress { get; set; }
        public bool ConvertToInvoice { get; set; }

        public CAddress BillingAddress { get; set; }

        public CAddress ShippingAddress { get; set; }

        public decimal Total { get; set; }

        public string Reference { get; set; }

        public string IssueNo { get; set; }

        public string Memo { get; set; }

        public DateTime Date { get; set; }

        public long CurrencyId { get; set; }

        public long? ClassId { get; set; }
        public long? TransactionTypeId { get; set; }

        public enumStatus.EnumStatus.TransactionStatus Status { get; set; }

        public List<CreateOrUpdateItemIssueItemInput> Items { get; set; }

        public bool IsConfirm { get; set; }
        public long? PermissionLockId { get; set; }

    }

    public class InvoiceSaleOrderDto {
        public Guid? orderItemId { get; set; }
        public Guid? soId { get; set; }
        public decimal sumTotalQty { get; set; }
        public decimal OriginalTotalQty { get; set; }
        public decimal qty { get; set; }

        public Guid? DeliveryItemId { get; set; }
        public Guid? DeliveryId { get; set; }
        public decimal DeliveryOriginalTotalQty { get; set; }
      
    }

    public class CreateItemIssueFromKitchenOrderInput {

        public ReceiveFrom ReceiveFrom { get; set; }
        public Guid? CustomerId { get; set; }
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
        public long? TransactionTypeId { get; set; }
        public Guid? KitchenOrderId { get; set; }
        public bool IsConfirm { get; set; }

        public enumStatus.EnumStatus.TransactionStatus Status { get; set; }

        public List<CreateOrUpdateItemIssueItemInput> Items { get; set; }
     
        public long? PermissionLockId { get; set; }


    }
    public class UpdateItemIssueFromKitchenOrderInput : CreateItemIssueFromKitchenOrderInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}

