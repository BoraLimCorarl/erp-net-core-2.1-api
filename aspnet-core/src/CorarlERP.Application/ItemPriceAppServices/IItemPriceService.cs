using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.ItemPriceAppServices.Dto;

namespace CorarlERP.ItemPriceAppServices
{
    public interface IItemPriceService : IApplicationService
    {
       
        Task<NullableIdDto<Guid>> Create(CreateItemPriceInput input);
        Task Delete(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdateItemPirceInput input);
        Task<PagedResultDto<GetSummanryItemPriceOutput>> GetList(GetItemPriceInput input);
        Task<PagedResultDto<GetSummanryItemPriceOutput>> Find(GetItemPriceInput input);
        Task<GetDetailItemPriceOutput> GetDetail(EntityDto<Guid> input);
        Task<FileDto> ExportExcel(Guid Id);
    }
}
