using System;
using System.Collections.Generic;
using Abp.AutoMapper;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Vendors.Dto;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.PaymentMethods.Dto
{
    [AutoMapFrom(typeof(PaymentMethod))]
    public class PaymentMethodDetailOutput
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public PaymentMethodBaseDetailOutput PaymentMethodBase { get; set; }
     
        public Guid AccountId { get; set; }
   
        public ChartAccountSummaryOutput Account { get; set; }
     
        public Member Member { get; set; }
        public List<GroupItems> UserGroups { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
    }

    public class PaymentMethodSummaryOutput
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
      
    }
}
