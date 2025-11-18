using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.POS.Dto
{
    public class POSPaymoreInput
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public virtual ICollection<POSPaymentSummaryByPaymentMethodInput> ReceivePayments { get; set; }
        public virtual ICollection<POSStoreCreditPaymentInput> StoreCreditPayments { get; set; }
        public bool IsConfirm { get; set; }
    }
}
