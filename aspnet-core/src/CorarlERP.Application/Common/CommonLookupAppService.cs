using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using CorarlERP.Common.Dto;
using CorarlERP.Editions;
using CorarlERP.Editions.Dto;
using static CorarlERP.Common.Dto.EnumStatus;
using System.Collections.Generic;
using CorarlERP.Journals.Dto;
using CorarlERP.ReportTemplates;
using CorarlERP.UserGroups.Dto;
using Abp.Application.Features;
using CorarlERP.Features;
using CorarlERP.PackageEditions;
using System;
using Abp.Application.Editions;

namespace CorarlERP.Common
{
    [AbpAuthorize]
    public class CommonLookupAppService : CorarlERPAppServiceBase, ICommonLookupAppService
    {
        private readonly EditionManager _editionManager;
        private readonly ICorarlRepository<PackageEdition, Guid> _packageEditionRepository;
        private readonly ICorarlRepository<Edition, int> _editionRepository;

        public CommonLookupAppService(EditionManager editionManager, ICorarlRepository<PackageEdition, Guid> packageEditionRepository, ICorarlRepository<Edition, int> editionRepository)
        {
            _editionManager = editionManager;
            _packageEditionRepository = packageEditionRepository;
            _editionRepository = editionRepository;
        }

        public async Task<ListResultDto<SubscribableEditionComboboxItemDto>> GetEditionsForCombobox(bool onlyFreeItems = false)
        {
            var subscribableEditions = (await _editionManager.Editions.Cast<SubscribableEdition>().ToListAsync())
                .WhereIf(onlyFreeItems, e => e.IsFree)
                .OrderBy(e => e.MonthlyPrice);

            return new ListResultDto<SubscribableEditionComboboxItemDto>(
                subscribableEditions.Select(e => new SubscribableEditionComboboxItemDto(e.Id.ToString(), e.DisplayName, e.IsFree, e.TrialDayCount, e.DailyPrice, e.IsPaid)).ToList()
            );
        }


        public async Task<ListResultDto<SubscribableEditionComboboxItemDto>> FindEditions(FindEditionInput input)
        {
          
            if (input.PackageId != null)
            {
                var subscribableEditions = await _packageEditionRepository.GetAll().Include(u=>u.Edition).Where(t => t.PackageId == input.PackageId)
                    .AsNoTracking()
                    .Select(d=> d.Edition).Cast<SubscribableEdition>()                   
                    .ToListAsync();
                return new ListResultDto<SubscribableEditionComboboxItemDto>(
                                                subscribableEditions.Select(e => new SubscribableEditionComboboxItemDto(e.Id.ToString(), e.DisplayName, e.IsFree, e.TrialDayCount, e.DailyPrice, e.IsPaid)).ToList()
                                            );
            }
            else
            {
               var subscribableEditions = (await _editionManager.Editions.Cast<SubscribableEdition>().ToListAsync())
                                          .OrderBy(e => e.MonthlyPrice);
                                            return new ListResultDto<SubscribableEditionComboboxItemDto>(
                                                subscribableEditions.Select(e => new SubscribableEditionComboboxItemDto(e.Id.ToString(), e.DisplayName, e.IsFree, e.TrialDayCount, e.DailyPrice, e.IsPaid)).ToList()
                                            );
            }
        }

        public async Task<PagedResultDto<NameValueDto>> FindUsers(FindUsersInput input)
        {
            if (AbpSession.TenantId != null)
            {
                //Prevent tenants to get other tenant's users.
                input.TenantId = AbpSession.TenantId;
            }

            using (CurrentUnitOfWork.SetTenantId(input.TenantId))
            {
                var query = UserManager.Users
                    .WhereIf(
                        !input.Filter.IsNullOrWhiteSpace(),
                        u =>
                            u.Name.Contains(input.Filter) ||
                            u.Surname.Contains(input.Filter) ||
                            u.UserName.Contains(input.Filter) ||
                            u.EmailAddress.Contains(input.Filter)
                    );

                var userCount = await query.CountAsync();
                var users = await query
                    .OrderBy(u => u.Name)
                    .ThenBy(u => u.Surname)
                    .PageBy(input)
                    .ToListAsync();

                return new PagedResultDto<NameValueDto>(
                    userCount,
                    users.Select(u =>
                        new NameValueDto(
                            u.FullName + " (" + u.EmailAddress + ")",
                            u.Id.ToString()
                            )
                        ).ToList()
                    );
            }
        }


        public async Task<PagedResultDto<GetDetailUserMember>> FindUserForUserGroup(FindUsersInput input)
        {
            if (AbpSession.TenantId != null)
            {
                //Prevent tenants to get other tenant's users.
                input.TenantId = AbpSession.TenantId;
            }

            using (CurrentUnitOfWork.SetTenantId(input.TenantId))
            {
                var query = UserManager.Users
                    .WhereIf(
                        !input.Filter.IsNullOrWhiteSpace(),
                        u =>
                            u.Name.Contains(input.Filter) ||
                            u.Surname.Contains(input.Filter) ||
                            u.UserName.Contains(input.Filter) ||
                            u.EmailAddress.Contains(input.Filter)
                    );

                var userCount = await query.CountAsync();
                var users = await query
                    .OrderBy(u => u.Name)
                    .ThenBy(u => u.Surname)
                    .Select(t => new GetDetailUserMember
                    {
                        Name = t.Name,
                        UserName = t.UserName,
                        SurName = t.Surname,
                        Id = t.Id
                    })
                    .PageBy(input)
                    .ToListAsync();
                return new PagedResultDto<GetDetailUserMember>(userCount, ObjectMapper.Map<List<GetDetailUserMember>>(users));
            }
        }

        public GetDefaultEditionNameOutput GetDefaultEditionName()
        {
            return new GetDefaultEditionNameOutput
            {
                Name = EditionManager.DefaultEditionName
            };
        }



        public List<Dto.GetStatusOutput> GetJournalStatus()
        {
            List<Dto.GetStatusOutput> result = new List<Dto.GetStatusOutput>() {
               // new Dto.GetStatusOutput{code = TransactionStatus.Draft.ToString(),Status = TransactionStatus.Draft},
                new Dto.GetStatusOutput{code =TransactionStatus.Publish.ToString(),Status = TransactionStatus.Publish},
                new Dto.GetStatusOutput{code =TransactionStatus.Void.ToString(),Status = TransactionStatus.Void},
                new Dto.GetStatusOutput{code =TransactionStatus.Close.ToString(),Status = TransactionStatus.Close},
            };

            return result;
        }

        public List<Dto.GetReportStatusOutput> GetReportStatus()
        {
            List<Dto.GetReportStatusOutput> result = new List<Dto.GetReportStatusOutput>() {
                new Dto.GetReportStatusOutput{code = ReportCategory.AccountingReport.ToString(),Status = ReportCategory.AccountingReport},
                new Dto.GetReportStatusOutput{code = ReportCategory.CustomerReport.ToString(),Status = ReportCategory.CustomerReport},
                new Dto.GetReportStatusOutput{code = ReportCategory.InventoryReport.ToString(),Status = ReportCategory.InventoryReport},
                new Dto.GetReportStatusOutput{code = ReportCategory.VendorReport.ToString(),Status = ReportCategory.VendorReport}
            };
            return result;
        }


        public async Task<List<GetJournalTypeStatus>> GetJournalType()
        {
            List<GetJournalTypeStatus> result = new List<GetJournalTypeStatus>() {
                new GetJournalTypeStatus{code = L("Bill"), Status = JournalType.Bill},
                new GetJournalTypeStatus{code = L("GeneralJournal"), Status = JournalType.GeneralJournal},
                new GetJournalTypeStatus{code = L("ItemReceipt_Purchase"), Status = JournalType.ItemReceiptPurchase},
                new GetJournalTypeStatus{code = L("ItemReceipt_Adjustment"), Status = JournalType.ItemReceiptAdjustment},
                new GetJournalTypeStatus{code = L("ItemReceipt_CustomerCredit"), Status = JournalType.ItemReceiptCustomerCredit},
                new GetJournalTypeStatus{code = L("ItemReceipt_Other"), Status = JournalType.ItemReceiptOther},
                new GetJournalTypeStatus{code = L("ItemReceipt_Production"), Status = JournalType.ItemReceiptProduction},
                new GetJournalTypeStatus{code = L("ItemReceipt_Transfer"), Status = JournalType.ItemReceiptTransfer},
                new GetJournalTypeStatus{code = L("ItemReceipt_PhysicalCount"), Status = JournalType.ItemReceiptPhysicalCount},
                new GetJournalTypeStatus{code = L("PayBill"), Status = JournalType.PayBill},
                new GetJournalTypeStatus{code = L("VendorCredit"), Status = JournalType.VendorCredit},
                new GetJournalTypeStatus{code = L("CustomerCredit"), Status = JournalType.CustomerCredit},
                new GetJournalTypeStatus{code = L("ReceivePayment"), Status = JournalType.ReceivePayment},
                new GetJournalTypeStatus{code = L("Invoice"), Status = JournalType.Invoice},
                new GetJournalTypeStatus{code = L("ItemIssue_Sale"), Status = JournalType.ItemIssueSale},
                new GetJournalTypeStatus{code = L("ItemIssue_Production"), Status = JournalType.ItemIssueProduction},
                new GetJournalTypeStatus{code = L("ItemIssue_Adjustment"), Status = JournalType.ItemIssueAdjustment},
                new GetJournalTypeStatus{code = L("ItemIssue_Other"), Status = JournalType.ItemIssueOther},
                new GetJournalTypeStatus{code = L("ItemIssue_Transfer"), Status = JournalType.ItemIssueTransfer},
                new GetJournalTypeStatus{code = L("ItemIssue_VendorCredit"), Status = JournalType.ItemIssueVendorCredit},
                new GetJournalTypeStatus{code = L("ItemIssue_PhysicalCount"), Status = JournalType.ItemIssuePhysicalCount},
                new GetJournalTypeStatus{code = L("Withdraw"), Status = JournalType.Withdraw},
                new GetJournalTypeStatus{code = L("Deposit"), Status = JournalType.Deposit},
               new GetJournalTypeStatus{code = L("ItemIssue_KitchenOrder"), Status = JournalType.ItemIssueKitchenOrder}
            }.OrderBy(t => t.code).ToList();


            if (!await FeatureChecker.IsEnabledAsync(AbpSession.TenantId.Value, AppFeatures.ProductionFeature))
            {
                result = result
                    .Where(t => t.Status != JournalType.ItemIssueProduction && t.Status != JournalType.ItemReceiptProduction).ToList();
            }
            if (!await FeatureChecker.IsEnabledAsync(AbpSession.TenantId.Value, AppFeatures.InventoryFeaturePhysicalCounts))
            {
                result = result
                    .Where(t => t.Status != JournalType.ItemIssuePhysicalCount && t.Status != JournalType.ItemReceiptPhysicalCount).ToList();
            }

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
    }
}
