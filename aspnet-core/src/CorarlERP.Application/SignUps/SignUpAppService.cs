using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using CorarlERP.SignUps.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorarlERP.Taxes.Dto;
using Abp.Collections.Extensions;
using System.Linq.Dynamic.Core;
using Abp.Authorization;
using CorarlERP.Authorization;
using CorarlERP.Configuration;
using CorarlERP.RedisCaches;
using Abp.Domain.Uow;
using Abp.Net.Mail;
using System.Text;
using CorarlERP.Emailing;
using CorarlERP.Authorization.ApiClients;
using CorarlERP.Security.Recaptcha;
using Abp.Timing;
using CorarlERP.FileStorages;
using CorarlERP.SubscriptionPayments.Dto;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot;
using Microsoft.AspNetCore.Mvc;
using CorarlERP.Referrals;
using CorarlERP.Subscriptions;
using CorarlERP.MultiTenancy;

namespace CorarlERP.SignUps
{
    public class SignUpAppService : CorarlERPAppServiceBase, ISignUpAppService
    {
        private readonly IApiClientManager _apiClientManager;
        private readonly ISignUpManager _signUpManager;
        private readonly IRepository<SignUp, Guid> _signUpRepository;
        private readonly IAppConfigurationAccessor _configuration;
        private readonly IRedisCacheManager _redisCacheServie;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IEmailTemplateProvider _emailTemplateProvider;
        private readonly ICorarlRepository<Referral, long> _referralRepository;
        private readonly ICorarlRepository<Subscription, Guid> _subScriptionRepository;
        private readonly ICorarlRepository<Tenant, int> _tenantRepository;
        public SignUpAppService(ISignUpManager signUpManager,
                             IRepository<SignUp, Guid> signUpRepository,
                             IAppConfigurationAccessor configuration,
                             IRedisCacheManager redisCacheServie,
                             IEmailSender emailSender,
                             IEmailTemplateProvider emailTemplateProvider,
                             IUnitOfWorkManager unitOfWorkManager,
                             IApiClientManager apiClientManager,
                             ICorarlRepository<Subscription, Guid> subScriptionRepository,
                             ICorarlRepository<Referral, long> referralRepository,
                             ICorarlRepository<Tenant, int> tenantRepository)
        {
            _signUpManager = signUpManager;
            _signUpRepository = signUpRepository;
            _configuration = configuration;
            _redisCacheServie = redisCacheServie;
            _emailSender = emailSender;
            _emailTemplateProvider = emailTemplateProvider;
            _unitOfWorkManager = unitOfWorkManager;
            _apiClientManager = apiClientManager;
            _referralRepository = referralRepository;
            _subScriptionRepository = subScriptionRepository;
            _tenantRepository = tenantRepository;
        }
        [AbpAuthorize(AppPermissions.Pages_Tenants_SignUps_Create)]
        public async Task<Guid> Create(CreateOrUpdateSignUpInput input)
        {
            var result = await CreateHelper(input, SignUp.EnumStatus.Pending);
            return result.Id;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_SignUps_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var @entity = await _signUpManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _signUpManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_SignUps_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var @entity = await _signUpManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _signUpManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_SignUps_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            var @entity = await _signUpManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _signUpManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_SignUps_Find)]
        public async Task<ListResultDto<GetDetailSignUpOutput>> Find(GetListSignUpInput input)
        {
            var @query = _signUpRepository
               .GetAll()
               .AsNoTracking()
               .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
               .WhereIf(
                   !input.Filter.IsNullOrEmpty(),
                     p => p.SignUpCode.ToLower().Contains(input.Filter.ToLower()) ||
                     p.FirstName.ToLower().Contains(input.Filter.ToLower()) ||
                     p.PhoneNumber.ToLower().Contains(input.Filter.ToLower()) ||
                     p.Position.ToLower().Contains(input.Filter.ToLower()) ||
                     p.CompanyOrStoreName.ToLower().Contains(input.Filter.ToLower()) ||
                     p.LastName.ToLower().Contains(input.Filter.ToLower())
                   )
               .OrderBy(p => p.SignUpCode);
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<GetDetailSignUpOutput>(resultCount, ObjectMapper.Map<List<GetDetailSignUpOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_SignUps_GetList)]
        public async Task<PagedResultDto<GetDetailSignUpOutput>> GetList(GetListSignUpInput input)
        {         
            var date = Clock.Now;
            var @query = from s in _signUpRepository
               .GetAll()
               .AsNoTracking()
               .Include(u => u.Referral)
               .Include(u => u.Tenant)
               .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)           
               .WhereIf(input.EnumStatus != null && input.EnumStatus.Count > 0, p => input.EnumStatus.Contains(p.Status))
               .WhereIf(input.Referrals != null && input.Referrals.Count > 0, p =>p.ReferralId != null && input.Referrals.Contains(p.ReferralId.Value))
               .WhereIf(
                   !input.Filter.IsNullOrEmpty(),
                     p => p.SignUpCode.ToLower().Contains(input.Filter.ToLower()) ||
                     p.Email.ToLower().Contains(input.Filter.ToLower()) ||
                     p.FirstName.ToLower().Contains(input.Filter.ToLower()) ||
                     p.PhoneNumber.ToLower().Contains(input.Filter.ToLower()) ||
                     p.Position.ToLower().Contains(input.Filter.ToLower()) ||
                     p.CompanyOrStoreName.ToLower().Contains(input.Filter.ToLower()) ||
                     p.LastName.ToLower().Contains(input.Filter.ToLower())
                   )
                         select new { signUp = s, tenant = s.Tenant };


            var results = from s in query
                          join su in _subScriptionRepository.GetAll().AsTracking()
                          on s.tenant.SubscriptionId equals su.Id into u
                          from d in u.DefaultIfEmpty()
                 
                          select new GetDetailSignUpOutput
                          {
                              CompanyOrStoreName = s.signUp.CompanyOrStoreName,
                              CreationTime = s.signUp.CreationTime,
                              Description = s.signUp.Description,
                              Email = s.signUp.Email,
                              FirstName = s.signUp.FirstName,
                              Id = s.signUp.Id,
                              IsActive = s.signUp.IsActive,
                              LastName = s.signUp.LastName,
                              PhoneNumber = s.signUp.PhoneNumber,
                              Position = s.signUp.Position,
                              SignUpCode = s.signUp.SignUpCode,
                              ReferralCode = s.signUp.Referral != null ? s.signUp.Referral.Code : null,
                              TenantId = s.signUp.TenantId,
                              Referral = s.signUp.Referral != null ? new ReferralSummaryOutput
                              {
                                  Code = s.signUp.Referral.Code,
                                  Id = s.signUp.ReferralId,
                                  Name = s.signUp.Referral.Name
                              } : new ReferralSummaryOutput { },
                              StartSubscriptionDate = s.signUp.Tenant != null ? s.tenant.CreationTime : s.signUp.CreationTime,
                              EndSubscriptionDate = d.Endate, //s.signUp.Tenant != null ? u.Where(b => b.Id == s.tenant.SubscriptionId).Select(sb => sb.Endate).FirstOrDefault() : (DateTime?)null,
                              CurrentDate = date,
                              StatusName = s.signUp.Status.ToString(),
                              Status = s.signUp.Status,
                          };



            var resultCount = await results.CountAsync();
            var @entities = await results
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<GetDetailSignUpOutput>(resultCount, ObjectMapper.Map<List<GetDetailSignUpOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_SignUps_GetList)]
        public async Task<GetDetailSignUpOutput> GetDetail(EntityDto<Guid> input)
        {
            var result = await _signUpRepository.GetAll().Include(u => u.Referral).Where(t => t.Id == input.Id).AsNoTracking().Select(t => new GetDetailSignUpOutput
            {
                CompanyOrStoreName = t.CompanyOrStoreName,
                Email = t.Email,
                FirstName = t.FirstName,
                Id = t.Id,
                IsActive = t.IsActive,
                LastName = t.LastName,
                PhoneNumber = t.PhoneNumber,
                Position = t.Position,
                SignUpCode = t.SignUpCode,
                CreationTime = t.CreationTime,
                ReferralCode = t.Referral.Code,
                Description = t.Description,
            }).FirstOrDefaultAsync();
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_SignUps_Edit)]
        public async Task<Guid> Update(CreateOrUpdateSignUpInput input)
        {
            var result = await _signUpRepository.GetAll().Where(t => t.Id == input.Id).AsNoTracking().FirstOrDefaultAsync();
            if (result == null) throw new UserFriendlyException(L("RecordNotFound"));
            result.Update(input.FirstName, input.LastName, input.Email, input.CompanyOrStoreName, input.Position, input.PhoneNumber, input.SignUpCode);
            result.SetReferralAndDescription(input.Description, result.ReferralId);
            await _signUpManager.UpdateAsync(result);
            return result.Id;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_SignUps_UpdateStatus)]
        public async Task UpdateStaus(UpdateStatusInput input)
        {
            var entitiy = await _signUpRepository.GetAll().Where(s => s.Id == input.Id).FirstOrDefaultAsync();
            entitiy.UpdateEnumStatus(input.Status);
            await _signUpRepository.UpdateAsync(entitiy);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenants_SignUps_UpdateStatus,AppPermissions.Pages_Tenants_SignUps)]
        public async Task<PagedResultDto<FindStatusOutput>> FindStatus()
        {
            var enumList = Enum.GetValues(typeof(SignUp.EnumStatus))
                                         .Cast<SignUp.EnumStatus>()
                                         .Select(e => new FindStatusOutput
                                         {
                                             Name = e.ToString(),
                                             Id = (int)e
                                         })
                                         .ToList();
            var resultCount = enumList.Count;
            return new PagedResultDto<FindStatusOutput>(resultCount, ObjectMapper.Map<List<FindStatusOutput>>(enumList));
        }
        [UnitOfWork(IsDisabled = true)]
        public async Task GenerateCodeAndPutToCache(GenerateTokenInput input)
        {
            var model = new ValidateInput
            {
                ClientId = input.ClientId,
                ClientSecret = input.ClientSecret
            };
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                await ValidateClientSecret(model);
                await CheckReferral(input.SignUpInput.ReferralCode);
            }
            if (input.SignUpInput == null)
            {
                throw new UserFriendlyException(L("SignUpInputIsRequired"));
            }
            else if (input.SignUpInput.FirstName == null)
            {
                throw new UserFriendlyException(L("FirstNameIsRequired"));
            }
            else if (input.SignUpInput.LastName == null)
            {
                throw new UserFriendlyException(L("LastNameIsRequired"));
            }
            else if (input.SignUpInput.CompanyOrStoreName == null)
            {
                throw new UserFriendlyException(L("CompanyOrStoreNameIsRequired"));
            }
            else if (input.SignUpInput.Position == null)
            {
                throw new UserFriendlyException(L("PositionIsRequired"));
            }
            else if (input.SignUpInput.PhoneNumber == null)
            {
                throw new UserFriendlyException(L("PhoneNumberIsRequired"));
            }

            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                await _signUpManager.CheckDuplicateCompanySignUp(input.SignUpInput.CompanyOrStoreName, input.SignUpInput.Email);
            }

            var autoMinuteToDelete = _configuration.Configuration["StackExchange:RedisCache:AutoMinuteToDelete"];
            var code = GenerateCode();
            await _redisCacheServie.SetDataAsync(input.Token, code, DateTime.Now.AddMinutes(Convert.ToDouble(autoMinuteToDelete)));
            var appName = _configuration.Configuration["App:Name"];
            await this.SendVerifyEmailAsync(code, input.SignUpInput.Email, appName, input.SignUpInput.FirstName, input.SignUpInput.LastName);
        }

        [UnitOfWork(IsDisabled = true)]
        public async Task VerifyCodeFromCache(VerifyTokenInput input)
        {
            var model = new ValidateInput
            {
                ClientId = input.ClientId,
                ClientSecret = input.ClientSecret
            };
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                await ValidateClientSecret(model);            
            }
            if (input.SignUpInput == null)
            {
                throw new UserFriendlyException(L("SignUpInputIsRequired"));
            }
            else if (input.SignUpInput.FirstName == null)
            {
                throw new UserFriendlyException(L("FirstNameIsRequired"));
            }
            else if (input.SignUpInput.LastName == null)
            {
                throw new UserFriendlyException(L("LastNameIsRequired"));
            }
            else if (input.SignUpInput.CompanyOrStoreName == null)
            {
                throw new UserFriendlyException(L("CompanyOrStoreNameIsRequired"));
            }
            else if (input.SignUpInput.Position == null)
            {
                throw new UserFriendlyException(L("PositionIsRequired"));
            }
            else if (input.SignUpInput.PhoneNumber == null)
            {
                throw new UserFriendlyException(L("PhoneNumberIsRequired"));
            }
            var mesasge = "";
            var result = await _redisCacheServie.GetDataAsync<string>(input.Token.ToString());
            if (result == null)
            {
                mesasge = "InvalidOTPCode";
                throw new UserFriendlyException(L(mesasge));
            }
            if (input.Code.ToLower() != result.ToLower())
            {
                mesasge = "InvalidOTPCode";
                throw new UserFriendlyException(L(mesasge));
            }
            await _redisCacheServie.RemoveDataAsync(input.Token);
            string referralName;
            using (var uow = _unitOfWorkManager.Begin(System.Transactions.TransactionScopeOption.RequiresNew))
            {
                var entities = await CreateHelper(input.SignUpInput, SignUp.EnumStatus.Pending);
                referralName = entities.ReferralName;
                await uow.CompleteAsync();
            }

            await SendTelegram(input.SignUpInput, referralName);
        }
        private async Task<long?> CheckReferral(string ReferralCode)
        {
            long? refeffalId = null;
            if (!string.IsNullOrWhiteSpace(ReferralCode))
            {
                var referral = await _referralRepository.GetAll().Where(s => s.IsActive && s.Code == ReferralCode).FirstOrDefaultAsync();
                var countReferral = await _signUpRepository.GetAll().Where(s => referral !=  null && s.ReferralId == referral.Id).CountAsync();
                var currentDate = DateTime.Now;
                refeffalId = referral != null ? referral.Id : (long?)null;
                if (referral == null)
                {
                    throw new UserFriendlyException(L("InvalidReferralCode", ReferralCode));
                }
                else if (referral.Code != null && referral.Qty <= countReferral)
                {
                    throw new UserFriendlyException(L("InvalidReferralQty", countReferral));
                }
                else if (referral.ExpirationDate != null && referral.ExpirationDate.Value.Date < currentDate.Date)
                {
                    throw new UserFriendlyException(L("InvalidReferralExpirationDate", referral.ExpirationDate.Value.ToString("dd/MM/yyyy")));
                }
            }
            return refeffalId;
        }
        private async Task SendTelegram(CreateOrUpdateSignUpInput input, string referralName)
        {

            var telegramBot = new TelegramBotClient(SignupBotToken);

            //var replyMarkup = new InlineKeyboardMarkup(new InlineKeyboardButton
            //{
            //    Text = "🔗 JOIN US",
            //    Url = SignupGroupUrl
            //});

            var description = $"<b>Signup Request</b>{Environment.NewLine}" +
                              $"Date: <b>{Clock.Now.ToString("yyyy-MM-dd HH:mm:ss")}</b>{Environment.NewLine}" +
                              $"First Name: <b>{input.FirstName}</b>{Environment.NewLine}" +
                              $"Last Name: <b>{input.LastName}</b>{Environment.NewLine}" +
                              $"Company: <b>{input.CompanyOrStoreName}</b>{Environment.NewLine}" +
                              $"Position: <b>{input.Position}</b>{Environment.NewLine}" +
                              $"Email: <b>{input.Email}</b>{Environment.NewLine}" +
                              $"Phone Number: <b>{input.PhoneNumber}</b>{Environment.NewLine}" +
                              $"Referral Name: <b>{referralName}</b> {Environment.NewLine}" +
                              $"Remark : <b>{input.Description}</b>";


            var sentMessage = await telegramBot.SendTextMessageAsync(
                chatId: $"{SignupGroup}",
                parseMode: ParseMode.Html,
                text: description
            );

        }


        #region helper
        private async Task<string> GetAutoSignCode()
        {
            var prefix = _configuration.Configuration["CustomerSignUp:Prefix"];
            var code = _configuration.Configuration["CustomerSignUp:SignUpCode"];
            prefix = prefix.IsNullOrEmpty() ? "" : prefix;
            var result = $"{prefix}{code}";
            var latestCode = await _signUpRepository
                .GetAll().AsNoTracking()
                .Where(s => s.SignUpCode.StartsWith(prefix) && s.SignUpCode.Length == prefix.Length + code.Length)
                .Select(s => s.SignUpCode)
                .OrderByDescending(s => s)
                .FirstOrDefaultAsync();

            if (!latestCode.IsNullOrWhiteSpace())
            {
                var index = prefix.IsNullOrEmpty() ? latestCode : latestCode.Replace(prefix, "");
                var number = Convert.ToInt32(index) + 1;

                result = $"{prefix}{number.ToString().PadLeft(code.Length, '0')}";

            }
            return result;
        }

        private string GenerateCode()
        {
            Random generator = new Random();
            string r = generator.Next(0, 1000000).ToString("D6");
            return r;
        }


        private async Task SendVerifyEmailAsync(string code, string email, string appName, string firstName, string lastName)
        {
            var logoLinkURL = _configuration.Configuration["App:LogoLinkURL"];
            var logoSourceURL = _configuration.Configuration["App:LogoSourceURL"];
            if (code.IsNullOrEmpty())
            {
                throw new Exception("EmailConfirmationCode should be set in order to send email activation link.");
            }
            var emailDetailInfoMessage = new StringBuilder("");

            emailDetailInfoMessage.AppendLine("<tr>");
            emailDetailInfoMessage.AppendLine("<td class='centerForSmall mobileFontSize'" +
                                                   "style='font - family:Arial, Helvetica, sans - serif; font - size:13px;" +
                                                   "color:#333333;mso-line-height-rule:exactly;line-height:24px;'>");
            emailDetailInfoMessage.AppendLine($"Use the following one-time password (OTP) to signup for a {appName} Account.");
            emailDetailInfoMessage.AppendLine("</td>");
            emailDetailInfoMessage.AppendLine("</tr>");

            emailDetailInfoMessage.AppendLine("<tr>");
            emailDetailInfoMessage.AppendLine("<td class='centerForSmall mobileFontSize'" +
                                                   "style='font - family:Arial, Helvetica, sans - serif; font - size:13px;" +
                                                   "color:#333333;mso-line-height-rule:exactly;line-height:24px;'>");

            emailDetailInfoMessage.AppendLine($"This OTP will be valid for 5 minutes till <b> {DateTime.Now.ToString("dd/MM/yyyy hh:mm tt")}</b> ");
            emailDetailInfoMessage.AppendLine("</td>");
            emailDetailInfoMessage.AppendLine("</tr>");
            var emailTemplate = GetSignUpTemplate();
            emailTemplate.Replace("{EMAIL_FULL_NAME_VALUE}", firstName);
            emailTemplate.Replace("{AppName}", appName);
            emailTemplate.Replace("{Code}", code);
            emailTemplate.Replace("{EMAIL_ACCOUNT_DETAIL_INFO}", emailDetailInfoMessage.ToString());
            emailTemplate.Replace("@LogoLinkURL", logoLinkURL);
            emailTemplate.Replace("@LogoSourceURL", logoSourceURL);
            var subject = L("EmailSignup_Corarl", appName);
            await _emailSender.SendAsync(email, subject, emailTemplate.ToString());
        }

        private StringBuilder GetSignUpTemplate()
        {
            var emailTemplate = new StringBuilder(_emailTemplateProvider.GetSignUpTemplate());

            return emailTemplate;
        }

        private async Task<CreateSignUpOutput> CreateHelper(CreateOrUpdateSignUpInput input, SignUp.EnumStatus status)
        {
            long? referralId = null;
            string referralName = null;
            if (!string.IsNullOrWhiteSpace(input.ReferralCode))
            {
                var referral = await _referralRepository.GetAll().Where(s => s.IsActive && s.Code == input.ReferralCode).FirstOrDefaultAsync();
                referralId = referral != null ? referral.Id : (long?)null;
                referralName = referral != null ? referral.Name : string.Empty;
            }
            input.SignUpCode = await GetAutoSignCode();
            var @entity = SignUp.Create(input.FirstName, input.LastName, input.Email, input.CompanyOrStoreName, input.Position, input.PhoneNumber, input.SignUpCode);
            entity.SetReferralAndDescription(input.Description, referralId);
            entity.UpdateEnumStatus(status);
            CheckErrors(await _signUpManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();
            return new CreateSignUpOutput
            {
                Id = entity.Id,
                ReferralId = referralId,
                ReferralName = referralName
            };
        }


        private async Task<ApplicationTypes> ValidateClientSecret(ValidateInput model)
        {
            return await _apiClientManager.ValidateClientSecretAsync(model.ClientSecret, model.ClientId);
        }

        #endregion


        [AbpAuthorize(AppPermissions.Pages_Tenants_SignUps_LinkTenant)]
        public async Task LinkTenant(LinkTenantInput input)
        {
            var @entity = await _signUpRepository.GetAll().Include(u => u.Tenant).Where(s => s.Id == input.Id).FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            @entity.UpdateTenant(input.TenantId);
            var subscriptionId =await _tenantRepository.GetAll().AsNoTracking().Where(s => s.Id == input.TenantId).Select(s => s.SubscriptionId).FirstOrDefaultAsync();
            var subscription = await _subScriptionRepository.GetAll().Where(s => s.Id == subscriptionId).FirstOrDefaultAsync();
            if (subscription.IsTrail)
            {
                entity.UpdateEnumStatus(SignUp.EnumStatus.FreeTrail);
            }
            else
            {
                entity.UpdateEnumStatus(SignUp.EnumStatus.Subscribed);
             }
            await _signUpManager.UpdateAsync(entity);
        }

    }
}
