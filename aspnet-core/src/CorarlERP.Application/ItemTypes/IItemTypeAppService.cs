using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.ItemTypes.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemTypes
{
   public interface IItemTypeAppService: IApplicationService
    {
        Task<ListResultDto<ItemTypeDetailOutput>> GetList(GetItemTypeListInput input);
        Task<ListResultDto<ItemTypeDetailOutput>> Find(GetItemTypeListInput input);
        Task Sync();
    }
}
