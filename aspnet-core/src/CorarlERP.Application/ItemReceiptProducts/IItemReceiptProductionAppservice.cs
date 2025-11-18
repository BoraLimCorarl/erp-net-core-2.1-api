using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.ItemReceiptProducts.Dto;
using CorarlERP.ItemReceiptTransfers.Dto;
using CorarlERP.Journals.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemReceiptProducts
{
    public interface IItemReceiptProductionAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateItemReceiptProductionInput input);
        Task<ItemReceiptProductionDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdateItemReceiptProductionInput input);
        //Task Delete(EntityDto<Guid> input);
    }
}
