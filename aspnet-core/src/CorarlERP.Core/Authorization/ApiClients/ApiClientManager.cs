using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using CorarlERP.MultiTenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Authorization.ApiClients
{
    public class ApiClientManager: DomainService, IApiClientManager
    {
        private readonly IRepository<ApiClient, Guid> _apiClientRepository;
        private readonly TenantManager _tenantManager;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiClientManager(IRepository<ApiClient, Guid> apiClientRepository,
                                TenantManager tenantManager,
                                IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _apiClientRepository = apiClientRepository;
            _tenantManager = tenantManager;
        }

        public async Task<ApiClient> GetAsync(Guid id, bool noTracking = true)
        {
            var entity = noTracking?await _apiClientRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(u=>u.Id == u.Id):
                         await _apiClientRepository.FirstOrDefaultAsync(u => u.Id == id);
            return entity;
        }

        public async Task<ApiClientWithTenant> GetWithTenantAsync(string clientId)
        {

            var entity = await (from a in _apiClientRepository.GetAll().AsNoTracking().Where(u => u.ClientId == clientId)
                        join t in _tenantManager.Tenants.AsNoTracking() on a.OwnedByTenantId equals t.Id into clientTenant
                        from at in clientTenant.DefaultIfEmpty()
                        select new ApiClientWithTenant
                        {
                            ApiClient = a,
                            Tenant = at
                        }).FirstOrDefaultAsync();
            
            return entity;
        }

        public async Task<ApiClient> GetAsync(string clientId, bool noTracking = true)
        {
            var entity = noTracking ? await _apiClientRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(u => u.ClientId == clientId) :
                        await _apiClientRepository.FirstOrDefaultAsync(u => u.ClientId == clientId);
            return entity;
            
        }

        public async Task<IdentityResult> CreateAsync(ApiClient token)
        {
            await _apiClientRepository.InsertAsync(token);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ApiClient token)
        {
            await _apiClientRepository.DeleteAsync(token);
            return IdentityResult.Success;

        }

        public async Task<IdentityResult> UpdateAsync(ApiClient token)
        {
            await _apiClientRepository.UpdateAsync(token);
            return IdentityResult.Success;

        }

        public async Task<IdentityResult> DeleteAsync(Guid tokenId)
        {
            var token = await GetAsync(tokenId);
            if (token == null)
                return IdentityResult.Failed(new IdentityError
                { Description = L("RecordNotFound") });

            await _apiClientRepository.DeleteAsync(token);
            return IdentityResult.Success;

        }

        public async Task<ApplicationTypes> ValidateClientSecretAsync(string clientSecret, string clientId)
        {

            #region Validate client id and secrete first
            ApiClientWithTenant clientWithTenant = null;


            clientWithTenant = await GetWithTenantAsync(clientId);
            if (clientWithTenant == null || clientWithTenant.ApiClient == null)
            {
                throw new UserFriendlyException(L("ThisClientIsNotRegisteredInTheSystem"));
            }

            if (clientWithTenant.ApiClient.ApplicationType == ApplicationTypes.Api ||
                    clientWithTenant.ApiClient.ApplicationType == ApplicationTypes.MobileApp)
            {
                if (string.IsNullOrWhiteSpace(clientId))
                {
                    throw new UserFriendlyException(L("ClientSecretIsRequired"));
                }

                var verificationResult = new PasswordHasher<String>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).VerifyHashedPassword(clientId, clientWithTenant.ApiClient.Secret, clientSecret);

                if (verificationResult != PasswordVerificationResult.Success)
                {
                    throw new UserFriendlyException(L("ClientSecretIsInvalid"));
                }

                //if (String.IsNullOrEmpty(model.ClientDevice))
                //{

                //    throw new UserFriendlyException(L("ClientDeviceMustBeProvided"));
                //}

                //if (String.IsNullOrEmpty(model.ClientIpAddress))
                //{

                //    throw new UserFriendlyException(L("ClientIpAddressIsRequired"));
                //}

            }
            else if (clientWithTenant.ApiClient.ApplicationType == ApplicationTypes.FrontEnd)
            {
                //for FrontEnd or Web, we need only client Id. We don't need them to pass the secrete 
                //BUT we need to check cors that we allow


                var clientRoomUri = GetClientRootAddress();
                if (clientRoomUri == null)
                {
                    throw new UserFriendlyException(L("InvalidClientURL"));
                }

                var allowedOrigin = clientWithTenant.ApiClient.AllowedOrigin;
                if (allowedOrigin == null)
                    return clientWithTenant.ApiClient.ApplicationType;

                var trimAllowedOrigina = allowedOrigin.Trim();

                if (allowedOrigin == "*") return clientWithTenant.ApiClient.ApplicationType;

                var allowedOrigins = trimAllowedOrigina.Split(",");
                var isALlowed = false;
                foreach (var r in allowedOrigins)
                {
                    var trimR = r.Trim();
                    var gSubDomain = "//*.";
                    var allowedAllSubdomain = trimR.Contains(gSubDomain);
                    trimR = trimR.Replace(gSubDomain, "//");

                    var clientRootHost = clientRoomUri.Host.Replace("www.", "");
                    var allowedDomains = new List<string> { ".com" };

                    var selectedDomain = allowedDomains.Where(u => clientRootHost.Contains(u)).FirstOrDefault();

                    var strSubstring = selectedDomain != null ? clientRootHost.Substring(clientRootHost.LastIndexOf(selectedDomain)) : null;
                    var subStringCount = strSubstring != null && clientRootHost.Contains(selectedDomain) ?
                                      strSubstring.Split(".").Length : 0;
                    var checkCount = subStringCount;
                    
                    if (allowedAllSubdomain)
                    {
                        var splittedClientRootHosts = clientRootHost.Split(".").ToList();
                        if (splittedClientRootHosts.Count > checkCount) splittedClientRootHosts.RemoveAt(0);
                        clientRootHost = String.Join(".", splittedClientRootHosts);
                    }


                    Uri uri = null;
                    if (Uri.TryCreate(trimR, UriKind.Absolute, out uri))
                    {

                        if (uri.Scheme == clientRoomUri.Scheme &&
                            uri.Host == clientRootHost &&
                            uri.Port == clientRoomUri.Port)
                        {
                            isALlowed = true;
                            break;
                        }
                    }
                }
                if (!isALlowed)
                    throw new UserFriendlyException(L("InvalidCORS"));
            }

            return clientWithTenant.ApiClient.ApplicationType;

            #endregion
        }

        private Uri GetClientRootAddress()
        {
            var req = _httpContextAccessor.HttpContext.Request;
            if (req == null) return null;
            var originURL = req.Headers["Origin"].FirstOrDefault();
            Uri uri = null;
            if (Uri.TryCreate(originURL, UriKind.Absolute, out uri))
            {
                return uri;
            }
            return null;

        }

    }
}
