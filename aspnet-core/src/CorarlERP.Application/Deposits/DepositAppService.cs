using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.AccountCycles;
using CorarlERP.Authorization;
using CorarlERP.AutoSequences;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Currencies;
using CorarlERP.Currencies.Dto;
using CorarlERP.Customers.Dto;
using CorarlERP.Deposits.Dto;
using CorarlERP.Journals;
using CorarlERP.Journals.Dto;
using CorarlERP.Locks;
using CorarlERP.Vendors;
using CorarlERP.Vendors.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Deposits
{
    [AbpAuthorize]
    public class DepositAppService : CorarlERPAppServiceBase, IDepositAppService
    {

        private readonly ICurrencyManager _currencyManager;
        private readonly IRepository<Currency, long> _currencyRepository;

        private readonly IVendorManager _vendorItemManager;
        private readonly IRepository<Vendor, Guid> _vendorRepository;
        
        private readonly IDepositItemManager _depositItemManager;
        private readonly IDepositManager _depositManager;

        private readonly IRepository<Deposit, Guid> _depositRepository;
        private readonly IRepository<DepositItem, Guid> _depositItemRepository;
        
        private readonly IJournalManager _journalManager;
        private readonly IRepository<Journal, Guid> _journalRepository;

        private readonly IJournalItemManager _journalItemManager;
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;

        private readonly IChartOfAccountManager _chartOfAccountManager;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;

        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;

        private readonly IRepository<Lock, long> _lockRepository;


        public DepositAppService(
            IJournalManager journalManager,
            IRepository<Journal, Guid> journalRepository,
            IJournalItemManager journalItemManager,
            IRepository<JournalItem, Guid> journalItemRepository,
            ICurrencyManager currencyManager,
            IRepository<Currency, long> currencyRepository,
            IChartOfAccountManager chartOfAccountManager,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            DepositManager depositManager,
            IRepository<Deposit, Guid> depositRepository,
            DepositItemManager depositItemManager,
            IRepository<DepositItem, Guid> depositItemRepository,
            VendorManager vendorManager,
            IRepository<Vendor, Guid> vendorRepository,
            IAutoSequenceManager autoSequenceManger,
            IRepository<Lock, long> lockRepository,
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            IRepository<AccountCycle,long> accountCycleRepository
            ): base(accountCycleRepository,null,null)
        {
            _journalManager = journalManager;
            _journalManager.SetJournalType(JournalType.Deposit);
            _journalRepository = journalRepository;
            _journalItemManager = journalItemManager;
            _journalItemRepository = journalItemRepository;

            _currencyRepository = currencyRepository;
            _currencyManager = currencyManager;
            _chartOfAccountManager = chartOfAccountManager;
            _chartOfAccountRepository = chartOfAccountRepository;
           
            _vendorRepository = vendorRepository;
            _vendorItemManager = vendorManager;

            _depositItemManager = depositItemManager;
            _depositItemRepository = depositItemRepository;
            _depositManager = depositManager;
            _depositRepository = depositRepository;
            _autoSequenceManager = autoSequenceManger;
            _autoSequenceRepository = autoSequenceRepository;
            _lockRepository = lockRepository;

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Deposits_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreateDepositInput input)
        {

            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                    .Where(t => (t.LockKey == TransactionLockType.BankTransaction)
                    && t.IsLock == true && t.LockDate != null && t.LockDate.Value.Date >= input.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            //validate items when create by none
            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }

            if (input.LocationId == null || input.LocationId == 0)
            {
                throw new UserFriendlyException(L("PleaseSelectLocation"));
            }
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Deposit);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.DepositNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            //insert to journal
            var @entity = Journal.Create(tenantId, userId, 
                                        input.DepositNo, 
                                        input.Date,
                                        input.Memo,
                                        input.Total,
                                        input.Total, 
                                        input.CurrencyId, 
                                        input.ClassId, 
                                        input.Reference,
                                        input.LocationId);
            entity.UpdateStatus(input.Status);

            //insert journal item into credit
            var clearanceJournalItem = JournalItem.CreateJournalItem(
                tenantId, userId, entity, input.BankAccountId, 
                input.Memo, input.Total, 0, PostingKey.Bank, null);

            //insert to deposit          
            var deposit = Deposit.Create(tenantId, userId, input.ReceiveFromVendorId, input.ReceiveFromCustomerId, input.Total);
            
            @entity.UpdateDeposit(deposit);

            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));
            CheckErrors(await _depositManager.CreateAsync(deposit));
          
            foreach (var i in input.Items)
            {
                //insert to deposit item
                var depositItem = DepositItem.Create(tenantId, userId, deposit.Id, i.AccountId, i.Qty, i.UnitCost, i.Total );

                CheckErrors(await _depositItemManager.CreateAsync(depositItem));

                //insert journal item into debit
                var inventoryJournalItem = JournalItem.CreateJournalItem(
                    tenantId, userId, entity, i.AccountId, 
                    i.Description, 0, i.Total, 
                    PostingKey.Clearance, 
                    depositItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));
                
            }

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.BankTransaction };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
            
            return new NullableIdDto<Guid>() { Id = deposit.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Deposits_Delete)]
        public async Task Delete(CarlEntityDto input)
        {
            var journal = await _journalRepository.GetAll()
                   .Include(u => u.Deposit)
                   .Where(u => u.JournalType == JournalType.Deposit && u.DepositId == input.Id)
                   .FirstOrDefaultAsync();

            //query get Deposit
            var @entity = journal.Deposit;

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Deposit);

            if (journal.JournalNo == auto.LastAutoSequenceNumber)
            {
                var jo = await _journalRepository.GetAll().Where(t => t.Id != journal.Id && t.JournalType == JournalType.Deposit)
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
                   && t.IsLock == true && t.LockDate != null && t.LockDate.Value.Date >= journal.Date.Date).AsNoTracking().CountAsync();

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

            //query get journal and delete
            journal.UpdateDeposit(null);

            //query get journal item and delete
            var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == journal.Id).ToListAsync();
            foreach (var ji in jounalItems)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(ji));
            }
            CheckErrors(await _journalManager.RemoveAsync(journal));

            //query get deposit item and delete 
            var depositItems = await _depositItemRepository.GetAll()
                .Where(u => u.DepositId == entity.Id).ToListAsync();

            foreach (var di in depositItems)
            {
                CheckErrors(await _depositItemManager.RemoveAsync(di));
            }
            CheckErrors(await _depositManager.RemoveAsync(entity));

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.BankTransaction };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Deposits_GetDetail)]
        public async Task<DepositDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @journal = await _journalRepository
                                   .GetAll()
                                   .Include(u => u.Deposit)
                                   .Include(u => u.Deposit.Vendor)
                                   .Include(u => u.Deposit.Customer)
                                   .Include(u => u.Class)
                                   .Include(u => u.Currency)
                                   .Include(u=>u.Location)
                                   .AsNoTracking()
                                   .Where(u => u.JournalType == JournalType.Deposit && u.DepositId == input.Id)
                                   .FirstOrDefaultAsync();

            if (@journal == null || @journal.Deposit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var clearanceAccount = await (_journalItemRepository.GetAll()
                                       .Include(u => u.Account)
                                       .AsNoTracking()
                                       .Where(u => u.JournalId == journal.Id &&
                                                u.Key == PostingKey.Bank && u.Identifier == null)
                                       .Select(u => u.Account)).FirstOrDefaultAsync();

            var depositItems = await _depositItemRepository.GetAll()
                .Include(u => u.Account)
                .Where(u => u.DepositId == input.Id)
                .Join(
                    _journalItemRepository.GetAll().Include(u => u.Account).AsNoTracking(),
                    u => u.Id, s => s.Identifier,
                (bItem, jItem) =>
                new CreateOrUpdateDepositItemInput()
                {
                    Id = bItem.Id,
                    Account = ObjectMapper.Map<ChartAccountSummaryOutput>(bItem.Account),
                    AccountId = bItem.AccountId,
                    Qty = bItem.Qty,
                    Total = bItem.Total,
                    Description = jItem.Description,
                    UnitCost = bItem.UnitCost,
                    CreationDate = jItem.CreationTime
                }).OrderBy( t=> t.CreationDate)
                .ToListAsync();

            var result = ObjectMapper.Map<DepositDetailOutput>(journal.Deposit);
            result.StatusCode = journal.Status;
            result.DepositNo = journal.JournalNo;
            result.Date = journal.Date;
            result.ClassId = journal.ClassId;
            result.CurrencyId = journal.CurrencyId;
            result.Reference = journal.Reference;
            result.Currency = ObjectMapper.Map<CurrencyDetailOutput>(journal.Currency);
            result.Class = ObjectMapper.Map<ClassSummaryOutput>(journal.Class);
            result.DepositItems = depositItems;
            result.BankAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(clearanceAccount);
            result.BankAccountId = clearanceAccount.Id;
            result.Memo = journal.Memo;
            result.ReceiveFromVendor = ObjectMapper.Map<VendorSummaryOutput>(journal.Deposit.Vendor);
            result.ReceiveFromCustomer = ObjectMapper.Map<CustomerSummaryOutput>(journal.Deposit.Customer);
            result.LocationId = journal.LocationId;
            result.LocationName = journal?.Location?.LocationName;
            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Deposits_Update)]
        public async Task<NullableIdDto<Guid>> Update(UpdateDepositInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // update Journal 
            var @journal = await _journalRepository
                              .GetAll()
                              .Where(u => u.JournalType == JournalType.Deposit && u.DepositId == input.Id)
                              .FirstOrDefaultAsync();
            await CheckClosePeriod(journal.Date, input.Date);
            journal.Update(tenantId, input.DepositNo, input.Date, input.Memo, input.Total, input.Total, input.CurrencyId, input.ClassId, input.Reference, input.Status,input.LocationId);
            journal.UpdateStatus(input.Status);
            //update account 
            var accountItem = await (_journalItemRepository.GetAll()
                                      .Where(u => u.JournalId == journal.Id &&
                                               u.Key == PostingKey.Bank && u.Identifier == null)).FirstOrDefaultAsync();
            accountItem.UpdateJournalItem(tenantId, input.BankAccountId, input.Memo, input.Total, 0);
           

            //update item Issue 
            var deposit = await _depositManager.GetAsync(input.Id, true);

            if (deposit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            if (input.LocationId == null || input.LocationId == 0)
            {
                throw new UserFriendlyException(L("PleaseSelectLocation"));
            }

            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                       .Where(t => t.IsLock == true && t.LockDate != null &&
                                       (t.LockDate.Value.Date >= input.DateCompare.Value.Date || t.LockDate.Value.Date >= input.Date.Date)
                                       && t.LockKey == TransactionLockType.BankTransaction).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            deposit.Update(tenantId, input.ReceiveFromVendorId, input.ReceiveFromCustomerId, input.Total);

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Deposit);
            CheckErrors(await _journalManager.UpdateAsync(@journal, auto.DocumentType));

            CheckErrors(await _journalItemManager.UpdateAsync(accountItem));
            CheckErrors(await _depositManager.UpdateAsync(deposit));


            //Update Item Issue Item and Journal Item
            var depositItems = await _depositItemRepository.GetAll().Where(u => u.DepositId == input.Id).ToListAsync();

            var journalItems = await (_journalItemRepository.GetAll()
                                  .Where(u => u.JournalId == journal.Id &&
                                         u.Key == PostingKey.Clearance && u.Identifier != null)
                                ).ToListAsync();

            var toDeleteDepositItem = depositItems.Where(u => !input.Items.Any(i => i.Id != null && i.Id == u.Id)).ToList();
            var toDeletejournalItem = journalItems.Where(u => !input.Items.Any(i => u.Identifier != null && i.Id != null && i.Id == u.Identifier)).ToList();

            if (input.Items.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }
            foreach (var c in input.Items)
            {
                if (c.Id != null) //update
                {
                    var dItem = depositItems.FirstOrDefault(u => u.Id == c.Id);
                    var journalItem = journalItems.FirstOrDefault(u => u.Identifier == c.Id);
                    if (dItem != null)
                    {
                        //new
                        dItem.Update(tenantId, c.AccountId, c.Qty, c.UnitCost, c.Total);
                        CheckErrors(await _depositItemManager.UpdateAsync(dItem));

                    }
                    if (journalItem != null)
                    {
                        journalItem.UpdateJournalItem(userId, c.AccountId, c.Description, 0, c.Total);
                        CheckErrors(await _journalItemManager.UpdateAsync(journalItem));
                    }

                }
                else if (c.Id == null) //create
                {
                    //insert to item Issue item
                    var itemIssueItem = DepositItem.Create(tenantId, userId, deposit.Id, c.AccountId,
                                                                 c.Qty, c.UnitCost, c.Total);
                    CheckErrors(await _depositItemManager.CreateAsync(itemIssueItem));

                    var inventoryJournalItem = JournalItem.CreateJournalItem(tenantId, userId, journal,
                        c.AccountId, c.Description, 0, c.Total, PostingKey.Clearance, itemIssueItem.Id);
                    CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));

                }

            }

            foreach (var t in toDeleteDepositItem.OrderBy(u => u.Id))
            {
                CheckErrors(await _depositItemManager.RemoveAsync(t));
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
            return new NullableIdDto<Guid>() { Id = deposit.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Deposits_UpdateStatusToDraft)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input)
        {
            //query select convertitemReceipt             
            var @entity = await _journalRepository
                               .GetAll()
                               .Include(u => u.Deposit)
                               .Where(u => u.JournalType == JournalType.Deposit && u.DepositId == input.Id)
                               .FirstOrDefaultAsync();

            if (entity.Deposit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            await ValidateBankTransfer(entity.Deposit);

            entity.UpdateStatusToDraft();

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Deposit);
            CheckErrors(await _journalManager.UpdateAsync(entity, auto.DocumentType));
            
            return new NullableIdDto<Guid>() { Id = entity.DepositId };
        }

        /***** Functionality for check if has bank transfer id *****/
        private async Task ValidateBankTransfer(Deposit deposit)
        {
            var @entity = await _depositRepository.GetAll().AsNoTracking()
                           .Where(u => u.Id == deposit.Id)
                           .FirstOrDefaultAsync();
            if (@entity.BankTransferId != null)
            {
                throw new UserFriendlyException(L("BankTransferMessage"));
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Deposits_UpdateStatusToPublish)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input)
        {
            var @entity = await _journalRepository
                                .GetAll()
                                .Include(u => u.Deposit)
                                .Where(u => u.JournalType == JournalType.Deposit && u.DepositId == input.Id)
                                .FirstOrDefaultAsync();
            if (entity.Deposit == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            entity.UpdatePublish();
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Deposit);
            CheckErrors(await _journalManager.UpdateAsync(entity, auto.DocumentType));
            
            return new NullableIdDto<Guid>() { Id = entity.DepositId };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Deposits_UpdateStatusToVoid)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input)
        {
            var @jounal = await _journalRepository.GetAll()
                .Include(u => u.Deposit)
                .Where(u => u.JournalType == JournalType.Deposit && u.DepositId == input.Id)
                .FirstOrDefaultAsync();
            if (@jounal == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            await ValidateBankTransfer(@jounal.Deposit);

            jounal.UpdateVoid();

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Deposit);
            CheckErrors(await _journalManager.UpdateAsync(jounal, auto.DocumentType));
            
            return new NullableIdDto<Guid>() { Id = jounal.DepositId };
        }

        //public Task<PagedResultDto<GetListDepositOutput>> Find(GetListDepositInput input)
        //{
        //    throw new NotImplementedException();
        //}
        //public Task<PagedResultDto<GetListDepositOutput>> GetList(GetListDepositInput input)
        //{
        //    throw new NotImplementedException();
        //}

    }
}
