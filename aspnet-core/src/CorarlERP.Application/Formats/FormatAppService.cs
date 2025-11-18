using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using CorarlERP.Formats.Dto;
using Microsoft.EntityFrameworkCore;
using Abp.Linq.Extensions;
using CorarlERP.Currencies.Dto;
using Abp.Authorization;
using CorarlERP.Authorization;
using System.Linq.Dynamic.Core;

namespace CorarlERP.Formats
{
    [AbpAuthorize]
    public class FormatAppService : CorarlERPAppServiceBase, IFormatAppService
    {
        private readonly IFormatManager _formatManager;
        private readonly IRepository<Format, long> _formatRepository;
        private readonly IDefaultValues _defaultValues;

        public FormatAppService(IFormatManager formatManager,
                          IRepository<Format, long> formatRepository,
                          IDefaultValues defaultValues)
        {
            _formatManager = formatManager;
            _formatRepository = formatRepository;
            _defaultValues = defaultValues;
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Formats_FindDate)]
        public async Task<ListResultDto<FormatDetailOutput>> FindDate(GetFormatListInput input)
        {
            var @entities = await _formatRepository
                   .GetAll()
                   .AsNoTracking()
                   .Where(p => p.Key != null && p.Key == "Date")
                   .WhereIf(
                       !input.Filter.IsNullOrEmpty(),
                       p => p.Key.ToLower().Contains(input.Filter.ToLower()) ||
                            p.Name.ToLower().Contains(input.Filter.ToLower())
                   )
                   .OrderBy(p => p.Name)
                   .ToListAsync();
            return new ListResultDto<FormatDetailOutput>(ObjectMapper.Map<List<FormatDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Formats_FindNumber)]
        public async Task<ListResultDto<FormatDetailOutput>> FindNumber(GetFormatListInput input)
        {
            var @entities = await _formatRepository
                .GetAll()
                .AsNoTracking()
                .Where(p=>p.Key != null && p.Key == "Number")
                .WhereIf(
                    !input.Filter.IsNullOrEmpty(),
                    p => p.Key.ToLower().Contains(input.Filter.ToLower()) ||
                         p.Name.ToLower().Contains(input.Filter.ToLower())
                )
                .OrderBy(p => p.Name)
                .ToListAsync();
            return new ListResultDto<FormatDetailOutput>(ObjectMapper.Map<List<FormatDetailOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Host_Formats_GetList)]
        public async Task<PagedResultDto<FormatDetailOutput>> GetList(GetFormatListInput input)
        {
            var query = _formatRepository
                .GetAll()
                .AsNoTracking()
                .WhereIf(
                    !input.Filter.IsNullOrEmpty(),
                    p => p.Key.ToLower().Contains(input.Filter.ToLower()) ||
                         p.Name.ToLower().Contains(input.Filter.ToLower())
                );

            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<FormatDetailOutput>(resultCount, ObjectMapper.Map<List<FormatDetailOutput>>(@entities));
        }
        [AbpAuthorize(AppPermissions.Pages_Host_Client_Formats_Sync)]
        public async Task Sync()
        {
            var defaultFormates = _defaultValues.Formats;

            var defaultFormatName = defaultFormates.Select(u => u.Name.ToLower());

            if (defaultFormatName.Distinct().Count() !=
                defaultFormatName.Count())
            {
                throw new UserFriendlyException(L("DuplicateFormatName"));
            }


            var existingFormats = await _formatRepository.GetAll().ToListAsync();
            var lookup = existingFormats.ToDictionary(u => u.Name);

            foreach (var c in defaultFormates)
            {
                if (lookup != null && lookup.ContainsKey(c.Name))
                {
                    //update
                    var @entity = lookup[c.Name];
                    entity.Update(null,c.Name,c.Key,c.Web);
                    CheckErrors(await _formatManager.UpdateAsync(@entity, false));
                }
                else
                {
                    //create new
                    var @entity = Format.Create(null, c.Name, c.Key, c.Web);
                    CheckErrors(await _formatManager.CreateAsync(@entity, false));
                }
            }

        }


    }
}
