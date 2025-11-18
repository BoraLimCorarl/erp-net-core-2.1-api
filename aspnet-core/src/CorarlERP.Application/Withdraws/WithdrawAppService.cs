using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Authorization;
using CorarlERP.AutoSequences;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Currencies;
using CorarlERP.Currencies.Dto;
using CorarlERP.Customers.Dto;
using CorarlERP.Journals;
using CorarlERP.Journals.Dto;
using CorarlERP.Locks;
using CorarlERP.MultiTenancy;
using CorarlERP.Vendors;
using CorarlERP.Vendors.Dto;
using CorarlERP.Withdraws.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Withdraws
{
    [AbpAuthorize]
    public class WithdrawAppService : CorarlERPAppServiceBase, IWithdrawAppService
    {

        private readonly ICurrencyManager _currencyManager;
        private readonly IRepository<Currency, long> _currencyRepository;

        private readonly IVendorManager _vendorManager;
        private readonly IRepository<Vendor, Guid> _vendorRepository;

        private readonly IRepository<Withdraw, Guid> _withdrawRepository;
        private readonly IRepository<WithdrawItem, Guid> _withdrawItemRepository;

        private readonly IWithdrawManager _withdrawManager;
        private readonly IWithdrawItemManager _withdrawItemManager;

        private readonly IJournalManager _journalManager;
        private readonly IRepository<Journal, Guid> _journalRepository;

        private readonly IJournalItemManager _journalItemManager;
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;

        private readonly IChartOfAccountManager _chartOfAccountManager;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;

        private readonly ITenantManager _tenantManager;
        private readonly IRepository<Tenant, int> _tenantRepository;

        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;

        private readonly IRepository<Lock, long> _lockRepository;

        public WithdrawAppService(IJournalManager journalManager, IJournalItemManager journalItemManager,
         ICurrencyManager currencyManager,
         IChartOfAccountManager chartOfAccountManager, IWithdrawManager WithdrawManager,
         IWithdrawItemManager withdrawItemManager,
         VendorManager vendorManager,
         IRepository<Vendor, Guid> vendorRepository,
         IRepository<Journal, Guid> journalRepository,
         IRepository<JournalItem, Guid> journalItemRepository,
         IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
         IRepository<Withdraw, Guid> withdrawRepository,
         IRepository<WithdrawItem, Guid> withdrawItemRepository,
         IRepository<Currency, long> currencyRepository,
         ITenantManager tenantManager,
         IRepository<Tenant, int> tenantRepository ,
         IAutoSequenceManager autoSequenceManger,
         IRepository<Lock, long> lockRepository,
         IRepository<AutoSequence, Guid> autoSequenceRepository,
         IRepository<AccountCycles.AccountCycle,long> accountCycleRepository
         ): base(accountCycleRepository,null,null)
        {
            _journalManager = journalManager;
            _journalManager.SetJournalType(JournalType.Withdraw);
            _journalRepository = journalRepository;
            _journalItemManager = journalItemManager;
            _journalItemRepository = journalItemRepository;
            _chartOfAccountManager = chartOfAccountManager;
            _chartOfAccountRepository = chartOfAccountRepository;
            _withdrawItemManager = withdrawItemManager;
            _withdrawItemRepository = withdrawItemRepository;
            _withdrawManager = WithdrawManager;
            _withdrawRepository = withdrawRepository;
            _vendorRepository = vendorRepository;
            _vendorManager = vendorManager;
            _currencyRepository = currencyRepository;
            _currencyManager = currencyManager;
            _tenantManager = tenantManager;
            _tenantRepository = tenantRepository;
            _autoSequenceManager = autoSequenceManger;
            _autoSequenceRepository = autoSequenceRepository;
            _lockRepository = lockRepository;
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Withdraws_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreateWithdrawInput input)
        {
            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                   .Where(t => (t.LockKey == TransactionLockType.BankTransaction)
                   && t.IsLock == true && t.LockDate.Value.Date >= input.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Withdraw);

            if (input.LocationId == null || input.LocationId == 0)
            {
                throw new UserFriendlyException(L("PleaseSelectLocation"));
            }
            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.WithdrawNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            //insert to journal
            var @entity =
                Journal.Create(tenantId, userId, input.WithdrawNo, input.Date,
                input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference,input.LocationId);
            entity.UpdateStatus(input.Status);

            //insert BankAccount journal item into credit
            var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, input.BankAccountId, input.Memo, 0, input.Total, PostingKey.Bank, null);

            //insert to withdraw          
            var withdraw = Withdraw.Create(tenantId, userId, input.VendorId, input.CustomerId, input.Total);
            @entity.UpdateWithdraw(withdraw);

            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));
            CheckErrors(await _withdrawManager.CreateAsync(withdraw));

            foreach (var i in input.WithdrawItems)
            {
                //insert to Withdraw item
                var withdrawItem = WithdrawItem.Create(tenantId, userId, withdraw, i.Description, i.Qty, i.UnitCost, i.Total);
                CheckErrors(await _withdrawItemManager.CreateAsync(withdrawItem));

                //insert inventory journal item into debit
                var inventoryJournalItem =
                    JournalItem.CreateJournalItem(tenantId, userId, entity, i.AccountId, i.Description, i.Total, 0, PostingKey.Clearance, withdrawItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));
            }
            await CurrentUnitOfWork.SaveChangesAsync();
            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.BankTransaction };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
            return new NullableIdDto<Guid>() { Id = withdraw.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Withdraws_Delete)]
        public async Task Delete(CarlEntityDto input)
        {
            var journal = await _journalRepository.GetAll()
                .Include(u => u.withdraw)
                .Where(u => u.JournalType == JournalType.Withdraw && u.WithdrawId == input.Id)
                .FirstOrDefaultAsync();
            //query get withdraw
            var @entity = journal.withdraw;


            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Withdraw);

            if (journal.JournalNo == auto.LastAutoSequenceNumber)
            {
                var jo = await _journalRepository.GetAll().Where(t => t.Id != journal.Id && t.JournalType == JournalType.Withdraw)
                    .OrderByDescending(t => t.CreationTime).FirstOrDefaultAsync();
                if (jo != null)
                {
                    auto.UpdateLastAutoSequenceNumber(jo.JournalNo);
                }
                else
                {
                    auto.UpdateLastAutoSequenceNumber(null);
                }
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                  .Where(t => (t.LockKey == TransactionLockType.BankTransaction)
                  && t.IsLock == true && t.LockDate.Value.Date >= journal.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            if (journal.Status == TransactionStatus.Publish)
            {
                var draftInput = new UpdateStatus();
                draftInput.Id = input.Id;
                await UpdateStatusToDraft(draftInput);
            }

            journal.UpdateWithdraw(null);

            //query get journal item and delete
            var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == journal.Id).ToListAsync();
            foreach (var ji in jounalItems)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(ji));
            }
            CheckErrors(await _journalManager.RemoveAsync(journal));

            //query get withdraw item and delete 
            var withdrawItems = await _withdrawItemRepository.GetAll()
                .Where(u => u.WithdrawId == entity.Id).ToListAsync();

            foreach (var bi in withdrawItems)
            {

                CheckErrors(await _withdrawItemManager.RemoveAsync(bi));

            }

            CheckErrors(await _withdrawManager.RemoveAsync(entity));
            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.BankTransaction };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Withdraws_GetDetail)]
        public async Task<WithdrawDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @journal = await _journalRepository
                               .GetAll()
                               .Include(u => u.withdraw)
                               .Include(u => u.Location)
                               .Include(u => u.Class)
                               .Include(u => u.Currency)
                               .Include(u => u.withdraw.Vendor)
                               .Include(u => u.withdraw.Customer)
                               .AsNoTracking()
                               .Where(u => u.JournalType == JournalType.Withdraw && u.WithdrawId == input.Id)
                               .FirstOrDefaultAsync();

            if (@journal == null || @journal.withdraw == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var bankAccount = await (_journalItemRepository.GetAll()
                                       .Include(u => u.Account)
                                       .AsNoTracking()
                                       .Where(u => u.JournalId == journal.Id &&
                                                u.Key == PostingKey.Bank && u.Identifier == null)
                                       .Select(u => u.Account)).FirstOrDefaultAsync();

            var withdrawItems = await _withdrawItemRepository.GetAll()
                .Where(u => u.WithdrawId == input.Id)
                .Join(_journalItemRepository.GetAll().Include(u => u.Account).AsNoTracking(), u => u.Id, s => s.Identifier,
                (bItem, jItem) =>
                new WithdrawItemDetailOutput()
                {
                    CreationTime = bItem.CreationTime,
                    Id = bItem.Id,
                    AccountId = jItem.AccountId,
                    Account = ObjectMapper.Map<ChartAccountSummaryOutput>(jItem.Account),
                    Qty = bItem.Qty,
                    Total = bItem.Total,
                    UnitCost = bItem.UnitCost,
                    Description = jItem.Description
                }).OrderBy(u => u.CreationTime)
                .ToListAsync();
            var result = ObjectMapper.Map<WithdrawDetailOutput>(journal.withdraw);
            result.WithdrawNo = journal.JournalNo;
            result.Date = journal.Date;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.Reference = journal.Reference;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.WithdrawItems = withdrawItems;
            result.BankAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(bankAccount);
            result.Vendor = ObjectMapper.Map<VendorSummaryOutput>(journal.withdraw.Vendor);
            result.Customer = ObjectMapper.Map<CustomerSummaryOutput>(journal.withdraw.Customer);
            result.BankAccountId = bankAccount.Id;
            result.Memo = journal.Memo;
            result.StatusCode = journal.Status;
            result.StatusName = journal.Status.ToString();
            result.LocationId = journal?.LocationId;
            result.LocationName = journal?.Location?.LocationName;
            return result;
        }

        /***** Functionality for check if has bank transfer id *****/
        private async Task ValidateBankTransfer(Withdraw withdraw)
        {
            var @entity = await _withdrawRepository.GetAll().AsNoTracking()
                           .Where(u => u.Id == withdraw.Id)
                           .FirstOrDefaultAsync();
            if (@entity.BankTransferId != null)
            {
                throw new UserFriendlyException(L("BankTransferMessage"));
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Withdraws_Update)]
        public async Task<NullableIdDto<Guid>> Update(UpdateWithdrawInput input)
        {
            //validate withdrawItem when create by none
            if (input.WithdrawItems.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }
            if (input.LocationId == null || input.LocationId == 0)
            {
                throw new UserFriendlyException(L("PleaseSelectLocation"));
            }

            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                      .Where(t => t.IsLock == true &&
                                      (t.LockDate.Value.Date >= input.DateCompare.Value.Date || t.LockDate.Value.Date >= input.Date.Date)
                                      && t.LockKey == TransactionLockType.BankTransaction).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // update Journal 
            var @journal = await _journalRepository
                              .GetAll()
                              .Where(u => u.JournalType == JournalType.Withdraw && u.WithdrawId == input.Id)
                              .FirstOrDefaultAsync();

            await CheckClosePeriod (journal.Date, input.Date);
            journal.Update(tenantId, input.WithdrawNo, input.Date, input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, 0,input.LocationId);
            journal.UpdateStatus(input.Status);

            //update Clearance account 
            var bankAccountItem = await (_journalItemRepository.GetAll()
                                      .Where(u => u.JournalId == journal.Id &&
                                               u.Key == PostingKey.Bank && u.Identifier == null)).FirstOrDefaultAsync();
            bankAccountItem.UpdateJournalItem(tenantId, input.BankAccountId, input.Memo, 0, input.Total);

            //update withdraw 
            var withdraw = await _withdrawManager.GetAsync(input.Id, true);

            if (withdraw == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            // calculate balance and update 

            withdraw.Update(tenantId, input.Total, input.VendorId, input.CustomerId);

            var autoWithdraw = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Withdraw);
            CheckErrors(await _journalManager.UpdateAsync(@journal, autoWithdraw.DocumentType));

            CheckErrors(await _journalItemManager.UpdateAsync(bankAccountItem));
            CheckErrors(await _withdrawManager.UpdateAsync(withdraw));


            //Update withdraw Item and Journal Item
            var withdrawItems = await _withdrawItemRepository.GetAll().Where(u => u.WithdrawId == input.Id).ToListAsync();

            var @BankJournalItems = await (_journalItemRepository.GetAll()
                                  .Where(u => u.JournalId == journal.Id &&
                                           u.Key == PostingKey.Clearance && u.Identifier != null)).ToListAsync();

            var toDeleteWithdrawItem = withdrawItems.Where(u => !input.WithdrawItems.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalItem = BankJournalItems.Where(u => !input.WithdrawItems.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();

            foreach (var c in input.WithdrawItems)
            {
                if (c.Id != null) //update
                {
                    var withdrawItem = withdrawItems.FirstOrDefault(u => u.Id == c.Id);
                    var journalItem = BankJournalItems.FirstOrDefault(u => u.Identifier == c.Id);
                    if (withdrawItem != null)
                    {
                        //new
                        withdrawItem.Update(tenantId, c.Description, c.Qty, c.UnitCost, c.Total);
                        CheckErrors(await _withdrawItemManager.UpdateAsync(withdrawItem));
                    }

                    if (journalItem != null)
                    {
                        journalItem.UpdateJournalItem(userId, c.AccountId, c.Description, c.Total, 0);
                        CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                    }


                }
                else if (c.Id == null) //create
                {
                    //insert to withdraw item
                    var withdrawItem = WithdrawItem.Create(tenantId, userId, withdraw, c.Description, c.Qty, c.UnitCost, c.Total);
                    CheckErrors(await _withdrawItemManager.CreateAsync(withdrawItem));
                    //insert inventory journal item into debit
                    var BankJournalItem =
                        JournalItem.CreateJournalItem(tenantId, userId, journal, c.AccountId, c.Description, c.Total, 0, PostingKey.Clearance, withdrawItem.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(BankJournalItem));

                }


            }
            foreach (var t in toDeleteWithdrawItem)
            {

                CheckErrors(await _withdrawItemManager.RemoveAsync(t));

            }
            foreach (var t in toDeletejournalItem)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(t));
            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.BankTransaction };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = withdraw.Id };

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Withdraws_UpdateStatusToDraft)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input)
        {
            var @entity = await _journalRepository
                               .GetAll()
                               .Include(u => u.withdraw)
                               .Where(u => u.JournalType == JournalType.Withdraw && u.WithdrawId == input.Id)
                               .FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            await ValidateBankTransfer(entity.withdraw);

            entity.UpdateStatusToDraft();
            var autoWithdraw = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Withdraw);
            CheckErrors(await _journalManager.UpdateAsync(entity, autoWithdraw.DocumentType));
            
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Withdraws_UpdateStatusToPublish)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input)
        {
            var @entity = await _journalRepository
                             .GetAll()
                             .Include(u => u.withdraw)
                             .Where(u => u.JournalType == JournalType.Withdraw && u.WithdrawId == input.Id)
                             .FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.UpdatePublish();

            var autoWithdraw = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Withdraw);
            CheckErrors(await _journalManager.UpdateAsync(entity, autoWithdraw.DocumentType));
            
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Withdraws_UpdateStatusToVoid)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input)
        {
            var @entity = await _journalRepository
                           .GetAll()
                           .Include(u => u.withdraw)
                           .Where(u => u.JournalType == JournalType.Withdraw && u.WithdrawId == input.Id)
                           .FirstOrDefaultAsync();

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            await ValidateBankTransfer(entity.withdraw);

            entity.UpdateVoid();

            var autoWithdraw = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Withdraw);
            CheckErrors(await _journalManager.UpdateAsync(entity, autoWithdraw.DocumentType));

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }
    }
}
