using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Promotions
{
    public class GetPromotionInput
    {
        public Guid? SubscriptionId { get; set; }
        public DateTime Date { get; set; }
        public Guid PackageId { get; set; }
    }
}
