using System.Threading.Tasks;
using CorarlERP.Security.Recaptcha;

namespace CorarlERP.Tests.Web
{
    public class FakeRecaptchaValidator : IRecaptchaValidator
    {
        public Task ValidateAsync(string captchaResponse)
        {
            return Task.CompletedTask;
        }
    }
}
