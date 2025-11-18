namespace CorarlERP.Emailing
{
    public interface IEmailTemplateProvider
    {
        string GetDefaultTemplate(int? tenantId);
        string GetActivationTemplate();
        string GetSignUpTemplate();

    }
}
