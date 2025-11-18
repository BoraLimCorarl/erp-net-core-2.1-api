using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.AccountTypes.Dto;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.AccountTypes
{
    public interface IAccountTypeAppService: IApplicationService
    {
        Task<AccountTypeDetailOutput> Create(CreateAccountTypeInput input);
        Task<PagedResultDto<AccountTypeDetailOutput>> GetList(GetAccountTypeListInput input);
        Task<ListResultDto<AccountTypeDetailOutput>> Find(GetAccountTypeListInput input);
        Task<AccountTypeDetailOutput> Update(UpateAccountTypeInput input);
        Task Delete(EntityDto<long> input);
        Task Enable(EntityDto<long> input);
        Task Disable(EntityDto<long> input);
        Task ImportExcel(FileDto input);
        Task<FileDto> ExportExcel();

        Task<ListResultDto<NameValueDto<SubAccountType>>> GetSubAccountTypes(long? accountTypeId);
    }
}
