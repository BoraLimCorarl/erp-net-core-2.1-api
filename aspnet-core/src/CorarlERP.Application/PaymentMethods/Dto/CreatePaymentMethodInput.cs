using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.PaymentMethods.Dto
{
    public class CreatePaymentMethodInput
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Member Member { get; set; }
        public List<GroupItems> UserGroups { get; set; }
        public CreatePaymentMethodBaseInput PaymentMethodBase { get; set; }
       
    }

}
