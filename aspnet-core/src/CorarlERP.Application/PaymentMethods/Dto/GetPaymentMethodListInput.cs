using System;
using Abp.Runtime.Validation;
using CorarlERP.Dto;

namespace CorarlERP.PaymentMethods.Dto
{
    public class GetPaymentMethodListInput : GetPaymentMethodBaseListInput, IShouldNormalize
    {
        public long? LocationId { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "Name";
            }
        }
    }

    public class GetPaymentMethodBaseListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool? IsActive { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "Name";
            }
        }
    }
}
