using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Users;

namespace CorarlERP.Configuration.Host.Dto
{
    public class SendTestEmailInput
    {
        [Required]
        [MaxLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }
        public string CompanyName { get; set; }
    }
}