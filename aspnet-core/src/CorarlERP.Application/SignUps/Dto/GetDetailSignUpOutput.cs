using Abp.AutoMapper;
using CorarlERP.Referrals;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.SignUps.Dto
{
    [AutoMapFrom(typeof(SignUp))]
    public class GetDetailSignUpOutput : CreateOrUpdateSignUpInput
    {

        public int? TenantId { get; set; }
        public DateTime? CreationTime { get; set; }
        public ReferralSummaryOutput Referral { get; set; }
        public DateTime? StartSubscriptionDate { get; set; }
        public DateTime? EndSubscriptionDate { get; set; }
        public DateTime CurrentDate { get; set; }
        public string StatusName { get; set; }
        public SignUp.EnumStatus Status { get; set; }

        public int? CountSubscritionDate
        {
            get
            {
                if (StartSubscriptionDate.HasValue)
                {
                    return (CurrentDate - StartSubscriptionDate.Value).Days;
                }
                else { return null; }


            }
        }


    }
    [AutoMapFrom(typeof(Referral))]
    public class ReferralSummaryOutput
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }


}
