using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using Abp.Authorization.Users;

namespace CorarlERP.Web.Models.TokenAuth
{
    public class AuthenticateModel: AuthenticationBaseModel
    {
        [Required]
        [MaxLength(AbpUserBase.MaxEmailAddressLength)]
        public string UserNameOrEmailAddress { get; set; }

        [Required]
        [MaxLength(AbpUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        public string TwoFactorVerificationCode { get; set; }

        public bool RememberClient { get; set; }

        public string TwoFactorRememberClientToken { get; set; }

        public bool? SingleSignIn { get; set; }

        public string ReturnUrl { get; set; }
        [DisableAuditing]
        public string CaptchaResponse { get; set; }

    }

    public class AuthenticateForMobileModel : AuthenticateModel
    {
        [Required]
        public string Subdomain { get; set; }
    }
}