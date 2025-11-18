using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Customers.Dto;
using CorarlERP.Dto;
using CorarlERP.LayaltyAndMemberships.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.LayaltyAndMemberships
{
   public interface ICardAppService : IApplicationService
    {
        Task<Guid> Create(CreateCardInput input);
        Task<PagedResultDto<GetListCardOutput>> GetList(GetListCardInput input);
        Task<PagedResultDto<GetListCardOutput>> Find(GetListCardInput input);
        Task<GetListCardOutput> GetDetail(EntityDto<Guid> input);
        Task<Guid> Update(UpdateCardInput input);
        Task Delete(EntityDto<Guid> input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);
        Task Deactivate(EntityDto<Guid> input);
        Task<FileDto> ExportExcel(GetListCardExcelInput input);
        Task ImportExcel(FileDto input);
        Task<GetCustomerByCardId> GetCustomerIdByCardId(GetCustomerByCardIdInput input);
    }
}
