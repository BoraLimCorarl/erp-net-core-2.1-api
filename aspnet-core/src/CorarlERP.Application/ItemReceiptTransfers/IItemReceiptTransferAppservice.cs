using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.ItemReceiptTransfers.Dto;
using CorarlERP.Journals.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemReceiptTransfers
{
    public interface IItemReceiptTransferAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateItemReceiptTransferInput input);
        Task<ItemReceiptTransferDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdateItemReceiptTransferInput input);
        //Task Delete(EntityDto<Guid> input);
    }
}
