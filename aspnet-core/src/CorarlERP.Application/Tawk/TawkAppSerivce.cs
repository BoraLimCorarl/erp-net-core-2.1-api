using Abp.Authorization;
using Abp.Dependency;
using CorarlERP.Authorization;
using CorarlERP.Authorization.ApiClients;
using CorarlERP.Configuration;
using CorarlERP.SignUps.Dto;
using CorarlERP.Tawk.Dto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Tawk
{
    [AbpAuthorize()]
    public class TawkAppService : CorarlERPAppServiceBase, ITawkAppService
    {
        private readonly IConfigurationRoot _appConfiguration;
        private readonly IApiClientManager _apiClientManager;
        public TawkAppService(
            IApiClientManager apiClientManager,
            IHostingEnvironment hostingEnvironment) 
        {
            _appConfiguration = hostingEnvironment.GetAppConfiguration();
            _apiClientManager = apiClientManager;
        }

        private string ComputeHmacSha256(string message, string key)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                byte[] hashBytes = hmac.ComputeHash(messageBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public async Task<TawkDto> GetTawk(ValidateInput input)
        {
            await _apiClientManager.ValidateClientSecretAsync(input.ClientSecret, input.ClientId);

            string apiKey = _appConfiguration["Tawk:APIKey"];
            var encryptKey = $"{_appConfiguration["Tawk:SaltText"]}-{AbpSession.TenantId ?? 0}-{AbpSession.UserId.Value}";
            var hash = ComputeHmacSha256(encryptKey, apiKey);        

            return new TawkDto
            {
                PropertyId = _appConfiguration["Tawk:PropertyId"],
                WidgetId = _appConfiguration["Tawk:WidgetId"],
                EncryptKey = _appConfiguration["Tawk:SaltText"], 
                Hash = hash
            }; 
        }
    }
}
