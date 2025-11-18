using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.SignUps.Dto
{
    public class GetListSignUpInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool? IsActive { get; set; }
        public bool? IsConvert { get; set; }
        public List<long> Referrals { get; set; }
        public List<SignUp.EnumStatus> EnumStatus { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "SignUpCode";
            }
        }
    }
}
