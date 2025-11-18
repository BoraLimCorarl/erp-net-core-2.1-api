using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Editions.Dto;

namespace CorarlERP.Common
{
    public interface ICommonLookupAppService : IApplicationService
    {
        Task<ListResultDto<SubscribableEditionComboboxItemDto>> GetEditionsForCombobox(bool onlyFreeItems = false);

        Task<PagedResultDto<NameValueDto>> FindUsers(FindUsersInput input);
        Task<PagedResultDto<GetDetailUserMember>> FindUserForUserGroup(FindUsersInput input);
        GetDefaultEditionNameOutput GetDefaultEditionName();

        Task<List<GetJournalTypeStatus>> GetJournalType();

        List<GetStatusOutput> GetJournalStatus();

        List<GetListPaidStatus> GetPaidStatus();

        List<GetListOfAccountType> GetAccountTypeCode();

        List<GetListInventoryTransactionType> GetInventoryTransactionType();

        Task<ListResultDto<SubscribableEditionComboboxItemDto>> FindEditions(FindEditionInput input);
    }
}