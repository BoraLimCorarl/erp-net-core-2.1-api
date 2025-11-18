using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.TransactionTypes.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Transactiontypes
{
    public interface ITransactionTypeAppService : IApplicationService
    {
        Task<TransactionTypeDetailOutput> Create(CreateTransactionTypeInput input);
        Task<PagedResultDto<TransactionTypeDetailOutput>> GetList(GetTransactionTypeListInput input);
        Task<PagedResultDto<TransactionTypeDetailOutput>> Find(GetTransactionTypeListInput input);
        Task<TransactionTypeDetailOutput> GetDetail(EntityDto<long> input);
        Task<TransactionTypeDetailOutput> Update(UpdateTransactionTypeInput input);
        Task Delete(EntityDto<long> input);
        Task Enable(EntityDto<long> input);
        Task Disable(EntityDto<long> input);
    }
}
