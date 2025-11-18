using System;
using System.Reflection;
using System.Text;
using Abp.Dependency;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp.Reflection.Extensions;
using CorarlERP.Url;

namespace CorarlERP.Emailing
{
    public class EmailTemplateProvider : IEmailTemplateProvider, ITransientDependency
    {
        private readonly IWebUrlService _webUrlService;

        public EmailTemplateProvider(
            IWebUrlService webUrlService)
        {
            _webUrlService = webUrlService;
        }

        public string GetDefaultTemplate(int? tenantId)
        {
            using (var stream = typeof(EmailTemplateProvider).GetAssembly().GetManifestResourceStream("CorarlERP.Emailing.EmailTemplates.default.html"))
            {
                var bytes = stream.GetAllBytes();
                var template = Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
                template = template.Replace("{THIS_YEAR}",DateTime.Now.Year.ToString());
                return template.Replace("{EMAIL_LOGO_URL}", GetTenantLogoUrl(tenantId));
            }
        }

        public string GetActivationTemplate()
        {
            using (var stream = typeof(EmailTemplateProvider).GetAssembly().GetManifestResourceStream("CorarlERP.Emailing.EmailTemplates.default-reactive-email.html"))
            {
                var bytes = stream.GetAllBytes();
                return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
            }
        }

        public string GetSignUpTemplate()
        {
            using (var stream = typeof(EmailTemplateProvider).GetAssembly().GetManifestResourceStream("CorarlERP.Emailing.EmailTemplates.sign-up-template.html"))
            {
                var bytes = stream.GetAllBytes();
                return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
            }
        }

        private string GetTenantLogoUrl(int? tenantId)
        {
            if (!tenantId.HasValue)
            {
                return _webUrlService.GetServerRootAddress().EnsureEndsWith('/') + "TenantCustomization/GetTenantLogo?skin=light";
            }

            return _webUrlService.GetServerRootAddress().EnsureEndsWith('/') + "TenantCustomization/GetTenantLogo?skin=light&tenantId=" + tenantId.Value;
        }
    }
}