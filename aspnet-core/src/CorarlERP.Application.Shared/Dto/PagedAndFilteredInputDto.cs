using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace CorarlERP.Dto
{
    public class PagedAndFilteredInputDto : IPagedResultRequest
    {
        [Range(1, CorarlERPConsts.MaxPageSize)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public string Filter { get; set; }

        public PagedAndFilteredInputDto()
        {
            MaxResultCount = CorarlERPConsts.DefaultPageSize;
        }
    }
}