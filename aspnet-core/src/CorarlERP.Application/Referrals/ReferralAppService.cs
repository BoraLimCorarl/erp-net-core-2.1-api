using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using CorarlERP.Referrals.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using Abp.UI;
using CorarlERP.Authorization;

namespace CorarlERP.Referrals
{
    [AbpAuthorize]
    public class ReferralAppService : CorarlERPAppServiceBase, IReferralAppService
    {
        private readonly IReferralManager _referralManager;
        private readonly IRepository<Referral, long> _referralRepository;
        public ReferralAppService(IReferralManager referralManager,
                             IRepository<Referral, long> referralRepository)
        {
            _referralManager = referralManager;
            _referralRepository = referralRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Referrals_Create)]
        public async Task<long> Create(CreateOrUpdateReferallInput input)
        {
            var userId = AbpSession.UserId;
            bool isDuplicate;
            string referralCode;
            do
            {
                referralCode = GenerateRandomAlphanumericCode(6);
                isDuplicate = await _referralRepository.GetAll().AsNoTracking()
                              .Where(u => u.Code.ToLower() == referralCode.ToLower())
                           .AnyAsync();
            }
            while (isDuplicate);
            input.Code = referralCode;
            var @entity = Referral.Create(userId, input.Name, input.Qty, input.ExpirationDate, input.Code, input.Description);
            CheckErrors(await _referralManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();
            return entity.Id;
        }
        private string GenerateRandomAlphanumericCode(int length)
        {
            var random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Referrals_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var @entity = await _referralManager.GetAsync(input.Id, true);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            CheckErrors(await _referralManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Referrals_Disable)]
        public async Task Disable(EntityDto<long> input)
        {
            var @entity = await _referralManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _referralManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Referrals_Enable)]
        public async Task Enable(EntityDto<long> input)
        {
            var @entity = await _referralManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _referralManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Referrals_Find)]
        public async Task<PagedResultDto<ReferallDetailOutput>> Find(GetReferallListInput input)
        {
            var @query = _referralRepository
                 .GetAll()
                 .AsNoTracking()
                 .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                 .WhereIf(
                     !input.Filter.IsNullOrEmpty(),
                     p => p.Name.ToLower().Contains(input.Filter.ToLower())
                            || p.Code.ToLower().Contains(input.Filter.ToLower()))
                 .OrderBy(p => p.Name);
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<ReferallDetailOutput>(resultCount, ObjectMapper.Map<List<ReferallDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Referrals_GetList)]
        public async Task<PagedResultDto<ReferallDetailOutput>> GetList(GetReferallListInput input)
        {
            var @query = _referralRepository
                 .GetAll()
                 .AsNoTracking()
                 .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                 .WhereIf(
                     !input.Filter.IsNullOrEmpty(),
                     p => p.Name.ToLower().Contains(input.Filter.ToLower())
                            || p.Code.ToLower().Contains(input.Filter.ToLower()))
                 .OrderBy(p => p.Name);
            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<ReferallDetailOutput>(resultCount, ObjectMapper.Map<List<ReferallDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Referrals_Edit)]
        public async Task<long> Update(CreateOrUpdateReferallInput input)
        {

            var userId = AbpSession.UserId;
            var @entity = await _referralManager.GetAsync(input.Id.Value, true);
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            entity.Update(userId.Value, input.Name, input.Qty, input.ExpirationDate, entity.Code, input.Description);
            CheckErrors(await _referralManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();
            return entity.Id;
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Referrals_GetDetail)]
        public async Task<ReferallDetailOutput> GetDetail(EntityDto<long> input)
        {
            var result = await _referralRepository.GetAll().AsNoTracking().Where(r => r.Id == input.Id).Select(r =>
                  new ReferallDetailOutput
                  {
                      Code = r.Code,
                      Description = r.Description,
                      ExpirationDate = r.ExpirationDate,
                      Id = r.Id,
                      IsActive = r.IsActive,
                      Name = r.Name,
                      Qty = r.Qty

                  }).FirstOrDefaultAsync();

            return result;
        }
    }
}
