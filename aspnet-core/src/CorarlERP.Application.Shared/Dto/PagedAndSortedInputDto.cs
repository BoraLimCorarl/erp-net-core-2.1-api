using Abp.Application.Services.Dto;

namespace CorarlERP.Dto
{
    public class PagedAndSortedInputDto : PagedInputDto, ISortedResultRequest
    {
        public string Sorting { get; set; }

        public PagedAndSortedInputDto()
        {
            MaxResultCount = CorarlERPConsts.DefaultPageSize;
        }
    }
}