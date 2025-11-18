using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Threading;
using CorarlERP.Common.Dto;
using CorarlERP.Editions.Dto;
using static CorarlERP.Common.Dto.EnumStatus;

namespace CorarlERP.Common
{
    public class ProxyCommonLookupAppService : ProxyAppServiceBase, ICommonLookupAppService
    {
        public async Task<ListResultDto<SubscribableEditionComboboxItemDto>> GetEditionsForCombobox(bool onlyFreeItems = false)
        {
            return await ApiClient.GetAsync<ListResultDto<SubscribableEditionComboboxItemDto>>(GetEndpoint(nameof(GetEditionsForCombobox)));
        }

        public async Task<PagedResultDto<NameValueDto>> FindUsers(FindUsersInput input)
        {
            return await ApiClient.PostAsync<PagedResultDto<NameValueDto>>(GetEndpoint(nameof(FindUsers)), input);
        }

        public GetDefaultEditionNameOutput GetDefaultEditionName()
        {
            return AsyncHelper.RunSync(() =>
                ApiClient.GetAsync<GetDefaultEditionNameOutput>(GetEndpoint(nameof(GetDefaultEditionName))));
        }

        public List<Dto.GetStatusOutput> GetJournalStatus()
        {
            List<Dto.GetStatusOutput> result = new List<Dto.GetStatusOutput>() {
                new Dto.GetStatusOutput{code = TransactionStatus.Draft.ToString(),Status = TransactionStatus.Draft},
                new Dto.GetStatusOutput{code = TransactionStatus.Publish.ToString(),Status = TransactionStatus.Publish},
                new Dto.GetStatusOutput{code = TransactionStatus.Void.ToString(),Status = TransactionStatus.Void},
            };

            return result;
        }

        public List<GetListPaidStatus> GetPaidStatus()
        {
            List<GetListPaidStatus> result = new List<GetListPaidStatus>() {
                new GetListPaidStatus{code = PaidStatuse.Pending.ToString(), Status = PaidStatuse.Pending},
                new GetListPaidStatus{code = PaidStatuse.Partial.ToString(), Status = PaidStatuse.Partial},
                new GetListPaidStatus{code = PaidStatuse.Paid.ToString(), Status = PaidStatuse.Paid}
            }.OrderBy(t => t.code).ToList();

            return result;
        }

        public List<GetListOfAccountType> GetAccountTypeCode()
        {
            List<GetListOfAccountType> result = new List<GetListOfAccountType>() {
                new GetListOfAccountType{code = TypeOfAccount.COGS.ToString(), Status = TypeOfAccount.COGS},
                new GetListOfAccountType{code = TypeOfAccount.CurrentAsset.ToString(), Status = TypeOfAccount.CurrentAsset},
                new GetListOfAccountType{code = TypeOfAccount.CurrentLiability.ToString(), Status = TypeOfAccount.CurrentLiability},
                new GetListOfAccountType{code = TypeOfAccount.Equity.ToString(), Status = TypeOfAccount.Equity},
                new GetListOfAccountType{code = TypeOfAccount.Expense.ToString(), Status = TypeOfAccount.Expense},
                new GetListOfAccountType{code = TypeOfAccount.FixedAsset.ToString(), Status = TypeOfAccount.FixedAsset},
                new GetListOfAccountType{code = TypeOfAccount.Income.ToString(), Status = TypeOfAccount.Income},
                new GetListOfAccountType{code = TypeOfAccount.LongTermLiability.ToString(), Status = TypeOfAccount.LongTermLiability}
            }.OrderBy(t => t.code).ToList();
            return result;
        }



        public List<GetListInventoryTransactionType> GetInventoryTransactionType()
        {
            List<GetListInventoryTransactionType> result = new List<GetListInventoryTransactionType>() {
                new GetListInventoryTransactionType{code = InventoryTransactionType.ItemIssueAdjustment.ToString(), Status = InventoryTransactionType.ItemIssueAdjustment},
                new GetListInventoryTransactionType{code = InventoryTransactionType.ItemIssueCustomerCredit.ToString(), Status = InventoryTransactionType.ItemIssueCustomerCredit},
                new GetListInventoryTransactionType{code = InventoryTransactionType.ItemIssueOther.ToString(), Status = InventoryTransactionType.ItemIssueOther},
                new GetListInventoryTransactionType{code = InventoryTransactionType.ItemIssueSale.ToString(), Status = InventoryTransactionType.ItemIssueSale},
                new GetListInventoryTransactionType{code = InventoryTransactionType.ItemIssueTransfer.ToString(), Status = InventoryTransactionType.ItemIssueTransfer},
                new GetListInventoryTransactionType{code = InventoryTransactionType.ItemIssueVendorCredit.ToString(), Status = InventoryTransactionType.ItemIssueVendorCredit},
                new GetListInventoryTransactionType{code = InventoryTransactionType.ItemReceiptAdjustment.ToString(), Status = InventoryTransactionType.ItemReceiptAdjustment},
                new GetListInventoryTransactionType{code = InventoryTransactionType.ItemReceiptCustomerCredit.ToString(), Status = InventoryTransactionType.ItemReceiptCustomerCredit},
                new GetListInventoryTransactionType{code = InventoryTransactionType.ItemReceiptOther.ToString(), Status = InventoryTransactionType.ItemReceiptOther},
                new GetListInventoryTransactionType{code = InventoryTransactionType.ItemReceiptPurchase.ToString(), Status = InventoryTransactionType.ItemReceiptPurchase},
                new GetListInventoryTransactionType{code = InventoryTransactionType.ItemReceiptTransfer.ToString(), Status = InventoryTransactionType.ItemReceiptTransfer},
                new GetListInventoryTransactionType{code = InventoryTransactionType.ItemReceiptVendorCredit.ToString(), Status = InventoryTransactionType.ItemReceiptVendorCredit}
            }.OrderBy(t => t.code).ToList();
            return result;
        }

        public List<GetJournalTypeStatus> GetJournalType()
        {
            throw new System.NotImplementedException();
        }

        public Task<PagedResultDto<GetDetailUserMember>> FindUserForUserGroup(FindUsersInput input)
        {
            throw new System.NotImplementedException();
        }
    }
}
