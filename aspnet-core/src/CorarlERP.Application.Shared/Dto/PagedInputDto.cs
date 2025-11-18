using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace CorarlERP.Dto
{
    public class PagedInputDto : IPagedResultRequest
    {
        [Range(1, CorarlERPConsts.MaxPageSize)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public PagedInputDto()
        {
            MaxResultCount = CorarlERPConsts.DefaultPageSize;
        }
    }
}