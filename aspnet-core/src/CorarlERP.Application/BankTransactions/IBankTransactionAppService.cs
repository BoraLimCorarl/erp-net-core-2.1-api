using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.BankTransactions.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.BankTransactions
{
  public  interface IBankTransactionAppService : IApplicationService
    {
        Task<PagedResultDto<GetlistBankTransactionOutput>> GetListBankTransaction(GetListBankTransactionInput input);
    }
}
