using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.PurchasePrices.Dto;

namespace CorarlERP.PurchasePrices
{
    public interface IPurchasePriceAppService : IApplicationService
    {

        Task<NullableIdDto<Guid>> Create(CreatePurchasePriceInput input);
        Task Delete(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdatePurchasePriceInput input);
        Task<PagedResultDto<GetSummanryPurchasePriceOutput>> GetList(GetPurchasePriceInput input);
        Task<PagedResultDto<GetSummanryPurchasePriceOutput>> Find(GetPurchasePriceInput input);
        Task<GetDetailPurchasePriceOutput> GetDetail(EntityDto<Guid> input);
        Task<FileDto> ExportExcel(Guid Id);
    }
}
