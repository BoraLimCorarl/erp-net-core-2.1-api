using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.JournalTransactionTypes.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.JournalTransactionTypes
{
   public  interface IJournalTransactionTypeAppService : IApplicationService
    {
        Task<Guid> Create(CreateJournalTransactionTypeInput input);
        Task<PagedResultDto<GetJournalTransactionTypeDetail>> GetList(GetListJournalTransactionTypeInput input);
        Task<PagedResultDto<GetJournalTransactionTypeDetail>> Find(GetListJournalTransactionTypeInput input);
        Task<GetJournalTransactionTypeOutput> GetDetail(EntityDto<Guid> input);
        Task<Guid> Update(CreateJournalTransactionTypeInput input);
        Task Delete(EntityDto<Guid> input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);
        Task<PagedResultDto<GetListTypeForJournalTransactionOutput>> FindType(GetListTypeForJournalTransactionInput input);
        Task<GetJournalTransactionTypeOutput> GetTransactionType(GetInventoryTypeNameInput input);
    }
}
