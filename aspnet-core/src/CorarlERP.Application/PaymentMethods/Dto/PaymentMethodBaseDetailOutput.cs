using System;
using Abp.AutoMapper;
using CorarlERP.ChartOfAccounts.Dto;

namespace CorarlERP.PaymentMethods.Dto
{
    [AutoMapFrom(typeof(PaymentMethodBase))]
    public class PaymentMethodBaseDetailOutput
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }


}
