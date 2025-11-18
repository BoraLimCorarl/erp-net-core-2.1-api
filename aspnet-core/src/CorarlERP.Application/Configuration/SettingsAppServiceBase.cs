using System.Threading.Tasks;
using Abp.Net.Mail;
using CorarlERP.Configuration.Host.Dto;

namespace CorarlERP.Configuration
{
    public abstract class SettingsAppServiceBase : CorarlERPAppServiceBase
    {
        private readonly IEmailSender _emailSender;

        protected SettingsAppServiceBase(
            IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        #region Send Test Email

        public async Task SendTestEmail(SendTestEmailInput input)
        {
            await _emailSender.SendAsync(
                input.EmailAddress,
                L("TestEmail_Subject", input.CompanyName),
                L("TestEmail_Body")
            );
        }

        #endregion
    }
}
