using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.BankTransfers.Dto;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Classes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Abp.Authorization;
using CorarlERP.Authorization;
using CorarlERP.Journals.Dto;
using CorarlERP.Deposits.Dto;
using CorarlERP.Journals;
using CorarlERP.Deposits;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.Withdraws;
using CorarlERP.Withdraws.Dto;
using CorarlERP.AutoSequences;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Dto;
using CorarlERP.UserGroups;
using CorarlERP.Locations;
using CorarlERP.Locations.Dto;
using CorarlERP.Locks;
using CorarlERP.AccountCycles;
using CorarlERP.Common.Dto;

namespace CorarlERP.BankTransfers
{
    [AbpAuthorize]
    public class BankTransferAppService : CorarlERPAppServiceBase, IBankTransferAppService
    {
        private readonly IBankTransferManager _bankTransferManager;
        private readonly IRepository<BankTransfer, Guid> _banktransferRepository;

        private readonly IChartOfAccountManager _chartOfAccountManager;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;

        private readonly IClassManager _classManager;
        private readonly IRepository<Class, long> _classRepository;

        private readonly IDepositItemManager _depositItemManager;
        private readonly IDepositManager _depositManager;

        private readonly IRepository<Deposit, Guid> _depositRepository;
        private readonly IRepository<DepositItem, Guid> _depositItemRepository;

        private readonly IJournalManager _journalManager;
        private readonly IRepository<Journal, Guid> _journalRepository;

        private readonly IJournalItemManager _journalItemManager;
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;

        private readonly IRepository<Withdraw, Guid> _withdrawRepository;
        private readonly IRepository<WithdrawItem, Guid> _withdrawItemRepository;

        private readonly IWithdrawManager _withdrawManager;
        private readonly IWithdrawItemManager _withdrawItemManager;

        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;

        private readonly IRepository<Lock, long> _lockRepository;
        public BankTransferAppService(
            IBankTransferManager bankTransferManager,
            IRepository<BankTransfer, Guid> bankTransferRepository,
            IChartOfAccountManager chartOfAccountManager,
            IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            IClassManager classManager,
            IRepository<Class, long> classRepository,
             IJournalManager journalManager,
            IRepository<Journal, Guid> journalRepository,
            IJournalItemManager journalItemManager,
            IRepository<JournalItem, Guid> journalItemRepository,
             DepositManager depositManager,
            IRepository<Deposit, Guid> depositRepository,
            DepositItemManager depositItemManager,
            IRepository<DepositItem, Guid> depositItemRepository,
            IWithdrawManager withdrawManager,
            IWithdrawItemManager withdrawItemManager,
            IRepository<Withdraw, Guid> withdrawRepository,
            IRepository<WithdrawItem, Guid> withdrawItemRepository,
            IAutoSequenceManager autoSequenceManger,
            IRepository<AutoSequence, Guid> autoSequenceRepository,
            IRepository<Lock, long> lockRepository,
             IRepository<Location, long> locationRepository,
            IRepository<AccountCycle, long> accountCycleRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository) : base(accountCycleRepository,userGroupMemberRepository, locationRepository)
        {
            _bankTransferManager = bankTransferManager;
            _classManager = classManager;
            _classRepository = classRepository;
            _chartOfAccountManager = chartOfAccountManager;
            _chartOfAccountRepository = chartOfAccountRepository;
            _banktransferRepository = bankTransferRepository;

            _journalManager = journalManager;
            _journalRepository = journalRepository;
            _journalItemManager = journalItemManager;
            _journalItemRepository = journalItemRepository;

            _depositItemManager = depositItemManager;
            _depositItemRepository = depositItemRepository;
            _depositManager = depositManager;
            _depositRepository = depositRepository;

            _withdrawItemManager = withdrawItemManager;
            _withdrawItemRepository = withdrawItemRepository;
            _withdrawManager = withdrawManager;
            _withdrawRepository = withdrawRepository;
            _autoSequenceManager = autoSequenceManger;
            _autoSequenceRepository = autoSequenceRepository;
            _lockRepository = lockRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Transfers_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreateBankTransferInput input)
        {
            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                  .Where(t => (t.LockKey == TransactionLockType.BankTransfer)
                  && t.IsLock == true && t.LockDate != null && t.LockDate.Value.Date >= input.BankTransferDate.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            if (input.FromLocationId == null || input.FromLocationId == 0 || input.ToLocationId == null || input.ToLocationId == 0)
            {
                throw new UserFriendlyException(L("PleaseSelectLocation"));
            }

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.BankTransferOrder);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.BankTransferNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = BankTransfer.Create(
                tenantId, 
                userId, 
                input.BankTransferFromAccountId,
                input.BankTransferToAccountId, 
                input.TransferFromClassId,
                input.TransferToClassId, 
                input.BankTransferNo, 
                input.BankTransferDate,
                input.Reference,
                input.Status, 
                input.Memo, 
                input.Amount,
                input.FromLocationId,
                input.ToLocationId);
            CheckErrors(await _bankTransferManager.CreateAsync(@entity));
            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.BankTransfer };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            if (input.Status == TransactionStatus.Publish)
            {
                var tenant = await GetCurrentTenantAsync();
                if (tenant.BankTransferAccountId == null)
                {
                    throw new UserFriendlyException(L("PleaseSetDefaultValueBankTransferAccount"));
                }

                var createWithdrawInput = new CreateWithdrawInput()
                {
                    IsConfirm = input.IsConfirm,
                    Memo = input.Memo,
                    WithdrawNo = input.BankTransferNo,
                    ClassId = input.TransferToClassId,
                    CurrencyId = tenant.CurrencyId.Value,
                    Date = input.BankTransferDate,
                    Reference = input.Reference,
                    Status = input.Status,
                    Total = input.Amount,
                    BankAccountId = input.BankTransferFromAccountId,
                    VendorId = null,
                    LocationId = input.FromLocationId,
                    WithdrawItems = new List<CreateOrUpdateWithdrawItemInput>()
                    {
                        new CreateOrUpdateWithdrawItemInput()
                        {
                            Description = null,
                            Id = null,
                            Qty = 1,
                            Total = input.Amount,
                            UnitCost = input.Amount,
                            AccountId = tenant.BankTransferAccountId.Value,
                        }
                    }

                };
                await CreateWithdraw(createWithdrawInput, entity.Id);

                var createDepositInput = new CreateDepositInput()
                {
                    IsConfirm= input.IsConfirm,
                    Memo = input.Memo,
                    BankAccountId = input.BankTransferToAccountId,
                    DepositNo = input.BankTransferNo,
                    ReceiveFromVendorId = null,
                    ClassId = input.TransferToClassId,
                    CurrencyId = tenant.CurrencyId.Value,
                    Date = input.BankTransferDate,
                    Reference = input.Reference,
                    Status = input.Status,
                    Total = input.Amount,
                    LocationId = input.ToLocationId,
                    Items = new List<CreateOrUpdateDepositItemInput>()
                    {
                        new CreateOrUpdateDepositItemInput()
                        {
                            UnitCost = input.Amount,
                            Total = input.Amount,
                            AccountId = tenant.BankTransferAccountId.Value,
                            Description = null,
                            Id = null,
                            Qty = 1,
                        }
                    }

                };
                await CreateDeposit(createDepositInput, entity.Id);


            }

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Transfers_Delete)]
        public async Task Delete(CarlEntityDto input)
        {
            var @entity = await _bankTransferManager.GetAsync(input.Id, true);

            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                 .Where(t => (t.LockKey == TransactionLockType.BankTransfer)
                 && t.IsLock == true && t.LockDate != null && t.LockDate.Value.Date >= entity.BankTransferDate.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.BankTransferOrder);

            if (entity.BankTransferNo == auto.LastAutoSequenceNumber)
            {
                var bank = await _banktransferRepository.GetAll().Where(t => t.Id != entity.Id)
                    .OrderByDescending(t => t.CreationTime).FirstOrDefaultAsync();
                if (bank != null)
                {
                    auto.UpdateLastAutoSequenceNumber(bank.BankTransferNo);
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

            if (entity.Status == TransactionStatus.Publish)
            {
                var draftInput = new BankStatus();
                draftInput.Id = input.Id;
                draftInput.IsConfirm = input.IsConfirm;
                await UpdateStatusToDraft(draftInput);
            }

            CheckErrors(await _bankTransferManager.RemoveAsync(@entity));

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.BankTransfer };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
        }


        public async Task<PagedResultDto<GetListBankTransferOutput>> Find(GetListBankTrasferInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var @query = _banktransferRepository
                           .GetAll()
                           .Include(u => u.BankTransferFromAccount)
                           .Include(u => u.BankTransferToAccount)
                           .Include(u => u.TransferFromClass)
                           .Include(u => u.TransferToClass)
                           .AsNoTracking()
                           .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.ToLocationId.Value) || userGroups.Contains(t.FromLocationId.Value))
                           .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.FromLocationId))
                           .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.ToLocationId))
                           .WhereIf(
                               !input.Filter.IsNullOrEmpty(),
                               p => p.BankTransferNo.ToLower().Contains(input.Filter.ToLower())
                           );

            var resultCount = await query.CountAsync();
            var @entities = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<GetListBankTransferOutput>(resultCount, ObjectMapper.Map<List<GetListBankTransferOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Transfers_GetDetail)]
        public async Task<GetDetailBankTransferOutput> GetDetail(EntityDto<Guid> input)
        {
            await TurnOffTenantFilterIfDebug();

            var @entity = await _bankTransferManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var result = ObjectMapper.Map<GetDetailBankTransferOutput>(@entity);

            result.ToLocationId = entity?.ToLocationId;
            result.ToLocationName = entity?.ToLocation?.LocationName;
            result.FromLocationId = entity?.FromLocationId;
            result.FromLocationName = entity?.FromLocation?.LocationName;
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Transfers_GetList)]
        public async Task<PagedResultDto<GetListBankTransferOutput>> GetList(GetListBankTrasferInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();

            var accountQuery = GetAccounts(new List<Guid>());
            var locationQuery = GetLocations(null, input.Locations);
            var userQuery = GetUsers(input.Users);
            //var classQuery = _classRepository.GetAll()
            //                 .AsNoTracking()
            //                 .Select(s => new ClassSummaryOutput
            //                 {
            //                     Id = s.Id,
            //                     ClassName = s.ClassName
            //                 });

            var btQuery = _banktransferRepository
                           .GetAll()         
                           .Where(s => input.FromDate.Date <= s.BankTransferDate.Date)
                           .Where(s => s.BankTransferDate.Date <= input.ToDate.Date)
                           .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.FromLocationId) || input.Locations.Contains(u.ToLocationId))
                           .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.ToLocationId.Value) || userGroups.Contains(t.FromLocationId.Value))
                           .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                           .WhereIf(!input.Filter.IsNullOrEmpty(),
                                    p => p.BankTransferNo.ToLower().Contains(input.Filter.ToLower()) ||
                                         p.Reference.ToLower().Contains(input.Filter.ToLower()))
                           .AsNoTracking()
                           .Select(t => new 
                           {
                               Amount = t.Amount,
                               BankTransferDate = t.BankTransferDate,                               
                               BankTransferNo = t.BankTransferNo,
                               Id = t.Id,
                               Reference = t.Reference,
                               Status = t.Status,
                               CreatorUserId = t.CreatorUserId,
                               BankTransferFromAccountId = t.BankTransferFromAccountId,
                               BankTransferToAccountId = t.BankTransferToAccountId,
                               FromLocationId = t.FromLocationId,
                               ToLocationid = t.ToLocationId,
                               TransferFromClassId = t.TransferFromClassId,
                               TransferToClassId = t.TransferToClassId,                             
                           });

            var query = from t in btQuery
                        join u in userQuery
                        on t.CreatorUserId equals u.Id
                        join fa in accountQuery
                        on t.BankTransferFromAccountId equals fa.Id
                        join ta in accountQuery
                        on t.BankTransferToAccountId equals ta.Id
                        join fl in locationQuery
                        on t.FromLocationId equals fl.Id
                        join tl in locationQuery
                        on t.ToLocationid equals tl.Id
                        //join fc in classQuery
                        //on t.TransferFromClassId equals fc.Id
                        //join tc in classQuery
                        //on t.TransferToClassId equals tc.Id
                        select new GetListBankTransferOutput
                        {
                            Amount = t.Amount,
                            BankTransferDate = t.BankTransferDate,
                            BankTransferFromAccount = new ChartAccountSummaryOutput { 
                                Id = fa.Id,
                                AccountName = fa.AccountName,
                            },
                            BankTransferFromAccountId = t.BankTransferFromAccountId,
                            BankTransferNo = t.BankTransferNo,
                            BankTransferToAccount = new ChartAccountSummaryOutput { 
                                Id = ta.Id,
                                AccountName = ta.AccountName
                            },
                            BankTransferToAccountId = t.BankTransferToAccountId,
                            User = new UserDto { 
                                Id = u.Id,
                                UserName = u.UserName
                            },
                            Id = t.Id,
                            Reference = t.Reference,
                            Status = t.Status,
                            TransferFromClass = new ClassSummaryOutput { 
                                Id = t.TransferFromClassId,
                                //ClassName = fc.ClassName
                            },
                            TransferFromClassId = t.TransferFromClassId,
                            TransferToClass = new ClassSummaryOutput { 
                                Id = t.TransferToClassId,
                                //ClassName = tc.ClassName
                            },
                            TransferToClassId = t.TransferToClassId,
                            LocationFrom = new LocationSummaryOutput { 
                                Id = fl.Id,
                                LocationName = fl.LocationName
                            },
                            LocationTo = new LocationSummaryOutput { 
                                Id = tl.Id,
                                LocationName = tl.LocationName
                            }
                        };

            var resultCount = await query.CountAsync();
            if (resultCount == 0) return new PagedResultDto<GetListBankTransferOutput>(resultCount, new List<GetListBankTransferOutput>());

            if (input.Sorting.EndsWith("DESC"))
            {
                if (input.Sorting.ToLower().StartsWith("banktransferdate"))
                {
                    query = query.OrderByDescending(s => s.BankTransferDate);
                }
                else if (input.Sorting.ToLower().StartsWith("banktransferno"))
                {
                    query = query.OrderByDescending(s => s.BankTransferNo);
                }
                else if (input.Sorting.ToLower().StartsWith("banktransferfromaccount"))
                {
                    query = query.OrderByDescending(s => s.BankTransferFromAccount.AccountName);
                }
                else if (input.Sorting.ToLower().StartsWith("banktransfertoaccount"))
                {
                    query = query.OrderByDescending(s => s.BankTransferToAccount);
                }
                else if (input.Sorting.ToLower().StartsWith("amount"))
                {
                    query = query.OrderByDescending(s => s.Amount);
                }
                else if (input.Sorting.ToLower().StartsWith("status"))
                {
                    query = query.OrderByDescending(s => s.Status);
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    query = query.OrderBy(input.Sorting);
                }
            }
            else
            {
                if (input.Sorting.ToLower().StartsWith("banktransferdate"))
                {
                    query = query.OrderBy(s => s.BankTransferDate);
                }
                else if (input.Sorting.ToLower().StartsWith("banktransferno"))
                {
                    query = query.OrderBy(s => s.BankTransferNo);
                }
                else if (input.Sorting.ToLower().StartsWith("banktransferfromaccount"))
                {
                    query = query.OrderBy(s => s.BankTransferFromAccount.AccountName);
                }
                else if (input.Sorting.ToLower().StartsWith("banktransfertoaccount"))
                {
                    query = query.OrderBy(s => s.BankTransferToAccount);
                }
                else if (input.Sorting.ToLower().StartsWith("amount"))
                {
                    query = query.OrderBy(s => s.Amount);
                }
                else if (input.Sorting.ToLower().StartsWith("status"))
                {
                    query = query.OrderBy(s => s.Status);
                }
                else
                {
                    //Order by input field is slower than lambda expression!
                    //Try to avoid unless we don't know field name
                    query = query.OrderBy(input.Sorting);
                }
            }

            var @entities = await query.PageBy(input).ToListAsync();

            return new PagedResultDto<GetListBankTransferOutput>(resultCount, ObjectMapper.Map<List<GetListBankTransferOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Transfers_GetList)]
        public async Task<PagedResultDto<GetListBankTransferOutput>> GetListOld(GetListBankTrasferInput input)
        {
            // get user by group member
            var userGroups = await GetUserGroupByLocation();
            var @query = _banktransferRepository
                           .GetAll()
                           .Include(u => u.BankTransferFromAccount)
                           .Include(u => u.BankTransferToAccount)
                           .Include(u => u.TransferFromClass)
                           .Include(u => u.TransferToClass)
                           .Include(u => u.CreatorUser)
                           .Include(u=>u.FromLocation)
                           .Include(u=>u.ToLocation)
                           .AsNoTracking()
                           .WhereIf(input.Locations != null && input.Locations.Count > 0, u => input.Locations.Contains(u.FromLocationId) || input.Locations.Contains(u.ToLocationId))
                           .WhereIf(userGroups != null && userGroups.Count > 0, t => userGroups.Contains(t.ToLocationId.Value) || userGroups.Contains(t.FromLocationId.Value))
                           .WhereIf(input.Users != null && input.Users.Count > 0, u => input.Users.Contains(u.CreatorUserId))
                           .WhereIf(
                               !input.Filter.IsNullOrEmpty(),
                               p => p.BankTransferNo.ToLower().Contains(input.Filter.ToLower()) ||
                                    p.Reference.ToLower().Contains(input.Filter.ToLower()) ||
                                    p.Memo.ToLower().Contains(input.Filter.ToLower())
                           ).Select(t=> new GetListBankTransferOutput {
                               Amount = t.Amount,
                               BankTransferDate = t.BankTransferDate,
                               BankTransferFromAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.BankTransferFromAccount),
                               BankTransferFromAccountId = t.BankTransferFromAccountId,
                               BankTransferNo = t.BankTransferNo,
                               BankTransferToAccount = ObjectMapper.Map<ChartAccountSummaryOutput>(t.BankTransferToAccount),
                               BankTransferToAccountId= t.BankTransferToAccountId,
                               User = ObjectMapper.Map<UserDto>(t.CreatorUser),
                               Id=t.Id,
                               Reference =t.Reference,
                               Status =t.Status,
                               TransferFromClass = ObjectMapper.Map<ClassSummaryOutput>(t.TransferFromClass),
                               TransferFromClassId = t.TransferFromClassId,
                               TransferToClass= ObjectMapper.Map<ClassSummaryOutput>(t.TransferToClass),
                               TransferToClassId =t.TransferToClassId,
                               LocationFrom = ObjectMapper.Map<LocationSummaryOutput>(t.FromLocation),
                               LocationTo = ObjectMapper.Map<LocationSummaryOutput>(t.ToLocation)

                           });
            var resultCount = await query.CountAsync();
            var @entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<GetListBankTransferOutput>(resultCount, ObjectMapper.Map<List<GetListBankTransferOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Transfers_Update)]
        public async Task<NullableIdDto<Guid>> Update(UpdateBankTransferInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                     .Where(t => t.IsLock == true && t.LockDate != null &&
                                     (t.LockDate.Value.Date >= input.DateCompare.Value.Date || t.LockDate.Value.Date >= input.BankTransferDate.Date)
                                     && t.LockKey == TransactionLockType.BankTransfer).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var @entity = await _bankTransferManager.GetAsync(input.Id, true); //this is vendor


            
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            if (input.FromLocationId == null || input.FromLocationId == 0 || input.ToLocationId == null || input.ToLocationId == 0)
            {
                throw new UserFriendlyException(L("PleaseSelectLocation"));
            }
            await CheckClosePeriod(entity.BankTransferDate, input.BankTransferDate);
            entity.Update(
                        userId,
                        input.BankTransferFromAccountId,
                        input.BankTransferToAccountId, input.TransferFromClassId,
                        input.TransferToClassId, input.BankTransferNo, input.BankTransferDate, input.Reference,
                        input.Status, input.Memo, input.Amount,input.FromLocationId,input.ToLocationId);
            CheckErrors(await _bankTransferManager.UpdateAsync(@entity));

            if (input.IsConfirm)
            {
                //var clearLostInput = new List<TransactionLockType> { TransactionLockType.BankTransfer };
                //await this.CleanTransactionLockPassword(clearLostInput);
                await this.CleanTransactionLockPassword(input.PermissionLockId);
            }
            await CurrentUnitOfWork.SaveChangesAsync();


            if (input.Status == TransactionStatus.Publish)
            {
                //query select sub id of withdraw and deposit

                var deposit = await _depositRepository.GetAll()
                    .Where(t => t.BankTransferId == input.Id).FirstOrDefaultAsync();

                var withdraw = await _withdrawRepository.GetAll()
                    .Where(t => t.BankTransferId == input.Id).FirstOrDefaultAsync();

                //var @DepositItemId = await _depositItemRepository.GetAll()
                //    .Include(u => u.Deposit).Where(t => t.Deposit.BankTransferId == input.Id)
                //    .ToListAsync();

                //var @withdrawItemId = await _withdrawItemRepository.GetAll()
                //    .Include(u => u.Withdraw)
                //    .Where(t => t.Withdraw.BankTransferId == input.Id)
                //    .ToListAsync();


                var tenant = await GetCurrentTenantAsync();
                if (tenant.BankTransferAccountId == null)
                {
                    throw new UserFriendlyException(L("PleaseSetDefaultValueBankTransferAccount"));
                }
                var createWithdrawInput = new UpdateWithdrawInput()
                {
                    IsConfirm = input.IsConfirm,
                    Id = withdraw.Id,
                    Memo = input.Memo,
                    WithdrawNo = input.BankTransferNo,
                    ClassId = input.TransferFromClassId,
                    LocationId = input.FromLocationId,
                    CurrencyId = tenant.CurrencyId.Value,
                    Date = input.BankTransferDate,
                    Reference = input.Reference,
                    Status = input.Status,
                    Total = input.Amount,
                    BankAccountId = input.BankTransferFromAccountId,
                    VendorId = null,
                    WithdrawItems = new List<CreateOrUpdateWithdrawItemInput>()
                    {
                        new CreateOrUpdateWithdrawItemInput()
                        {
                            Description = null,
                            Id = null,
                            Qty = 1,
                            Total = input.Amount,
                            UnitCost = input.Amount,
                            AccountId = tenant.BankTransferAccountId.Value,
                        }
                    }

                };
                await UpdateWithdraw(createWithdrawInput);


                var createDepositInput = new UpdateDepositInput()
                {
                    IsConfirm  = input.IsConfirm,
                    Id = deposit.Id,
                    Memo = input.Memo,
                    BankAccountId = input.BankTransferToAccountId,
                    DepositNo = input.BankTransferNo,
                    ReceiveFromVendorId = null,
                    ClassId = input.TransferToClassId,
                    CurrencyId = tenant.CurrencyId.Value,
                    Date = input.BankTransferDate,
                    Reference = input.Reference,
                    Status = input.Status,
                    Total = input.Amount,
                    LocationId = input.ToLocationId,
                    Items = new List<CreateOrUpdateDepositItemInput>()
                    {
                        new CreateOrUpdateDepositItemInput()
                        {
                            UnitCost = input.Amount,
                            Total = input.Amount,
                            AccountId = tenant.BankTransferAccountId.Value,
                            Description = null,
                            Id = null,
                            Qty = 1,
                        }
                    }

                };
                await UpdateDeposit(createDepositInput);
            }


            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        private async Task CreateDeposit(CreateDepositInput input, Guid bankTransferId)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

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

            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Deposit);
            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.DepositNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }
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
            entity.UpdateCreationTimeIndex(entity.CreationTimeIndex.Value + 1);

            //insert journal item into credit
            var clearanceJournalItem = JournalItem.CreateJournalItem(
                                                tenantId, userId, entity, input.BankAccountId,
                                                input.Memo, input.Total, 0, PostingKey.Bank, null);

            //insert to deposit          
            var deposit = Deposit.Create(tenantId, userId, input.ReceiveFromVendorId, input.ReceiveFromCustomerId, input.Total);
            deposit.UpdateBankTransferId(bankTransferId);

            @entity.UpdateDeposit(deposit);

            CheckErrors(await _journalManager.CreateAsync(@entity, false, auto.RequireReference));
            CheckErrors(await _journalItemManager.CreateAsync(clearanceJournalItem));
            CheckErrors(await _depositManager.CreateAsync(deposit));

            foreach (var i in input.Items)
            {
                //insert to deposit item
                var depositItem = DepositItem.Create(tenantId, userId, deposit.Id, i.AccountId, i.Qty, i.UnitCost, i.Total);

                CheckErrors(await _depositItemManager.CreateAsync(depositItem));

                //insert journal item into debit
                var inventoryJournalItem = JournalItem.CreateJournalItem(
                                                        tenantId, userId, entity, i.AccountId,
                                                        i.Description, 0, i.Total,
                                                        PostingKey.Clearance,
                                                        depositItem.Id);
                CheckErrors(await _journalItemManager.CreateAsync(inventoryJournalItem));
            }
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        private async Task CreateWithdraw(CreateWithdrawInput input, Guid bankTransferId)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            //insert to journal

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
            var auto = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Withdraw);

            if (auto.CustomFormat == true)
            {
                var newAuto = _autoSequenceManager.GetNewReferenceNumber(auto.DefaultPrefix, auto.YearFormat.Value,
                               auto.SymbolFormat, auto.NumberFormat, auto.LastAutoSequenceNumber, DateTime.Now);
                input.WithdrawNo = newAuto;
                auto.UpdateLastAutoSequenceNumber(newAuto);
                CheckErrors(await _autoSequenceManager.UpdateAsync(auto));
            }

            var @entity = Journal.Create(tenantId, userId, input.WithdrawNo, input.Date,
                                            input.Memo, input.Total, input.Total, input.CurrencyId, 
                                            input.ClassId, input.Reference,
                                            input.LocationId);
            entity.UpdateStatus(input.Status);

            //insert BankAccount journal item into credit
            var clearanceJournalItem = JournalItem.CreateJournalItem(tenantId, userId, entity, input.BankAccountId, input.Memo, 0, input.Total, PostingKey.Bank, null);

            //insert to withdraw          
            var withdraw = Withdraw.Create(tenantId, userId, input.VendorId, input.CustomerId, input.Total);
            withdraw.UpdateBankTransferId(bankTransferId);
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
        }
        
        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Transfers_UpdateStatusToPublish)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input)
        {
            var @entity = await _bankTransferManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            entity.UpdateStatusToPublish();
            CheckErrors(await _bankTransferManager.UpdateAsync(entity));

            // auto create deposit and withdraw
            var tenant = await GetCurrentTenantAsync();
            if (tenant.BankTransferAccountId == null)
            {
                throw new UserFriendlyException(L("PleaseSetDefaultValueBankTransferAccount"));
            }

            var createDepositInput = new CreateDepositInput()
            {
                Memo = entity.Memo,
                BankAccountId = entity.BankTransferToAccountId,
                DepositNo = entity.BankTransferNo,
                ReceiveFromVendorId = null,
                ClassId = entity.TransferToClassId,
                CurrencyId = tenant.CurrencyId.Value,
                Date = entity.BankTransferDate,
                Reference = entity.Reference,
                Status = entity.Status,
                Total = entity.Amount,
                Items = new List<CreateOrUpdateDepositItemInput>()
                {
                    new CreateOrUpdateDepositItemInput()
                    {
                        UnitCost = entity.Amount,
                        Total = entity.Amount,
                        AccountId = tenant.BankTransferAccountId.Value,
                        Description = null,
                        Id = null,
                        Qty = 1,
                    }
                }

            };
            await CreateDeposit(createDepositInput, entity.Id);

            var createWithdrawInput = new CreateWithdrawInput()
            {
                Memo = entity.Memo,
                WithdrawNo = entity.BankTransferNo,
                ClassId = entity.TransferToClassId,
                CurrencyId = tenant.CurrencyId.Value,
                Date = entity.BankTransferDate,
                Reference = entity.Reference,
                Status = entity.Status,
                Total = entity.Amount,
                BankAccountId = entity.BankTransferFromAccountId,
                VendorId = null,
                WithdrawItems = new List<CreateOrUpdateWithdrawItemInput>()
                {
                    new CreateOrUpdateWithdrawItemInput()
                    {
                        Description = null,
                        Id = null,
                        Qty = 1,
                        Total = entity.Amount,
                        UnitCost = entity.Amount,
                        AccountId = tenant.BankTransferAccountId.Value,
                    }
                }

            };
            await CreateWithdraw(createWithdrawInput, entity.Id);

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Transfers_UpdateStatusToVoid)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input)
        {
            var @entity = await _bankTransferManager.GetAsync(input.Id, true);
            
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            if (entity.Status == TransactionStatus.Publish)
            {
                var withdrawId = _withdrawRepository.GetAll().Where(g => g.BankTransferId == input.Id).Select(t => t.Id).FirstOrDefault();
                var journalWithdraw = await _journalRepository
                                           .GetAll()
                                           .Include(u => u.withdraw)
                                           .Where(u => u.JournalType == JournalType.Withdraw && u.WithdrawId == withdrawId)
                                           .FirstOrDefaultAsync();

                if (journalWithdraw == null)
                {
                    throw new UserFriendlyException(L("RecordNotFound"));
                }

                journalWithdraw.UpdateVoid();

                var autoWithdraw = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Withdraw);
                CheckErrors(await _journalManager.UpdateAsync(journalWithdraw, autoWithdraw.DocumentType));
                

                var depositId = _depositRepository.GetAll().Where(g => g.BankTransferId == input.Id).Select(t => t.Id).FirstOrDefault();
                var jounalDeposit = await _journalRepository.GetAll()
                                            .Include(u => u.Deposit)
                                            .Where(u => u.JournalType == JournalType.Deposit && u.DepositId == depositId)
                                            .FirstOrDefaultAsync();
                if (jounalDeposit == null)
                {
                    throw new UserFriendlyException(L("RecordNotFound"));
                }
                jounalDeposit.UpdateVoid();

                var autoDeposit = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Deposit);
                CheckErrors(await _journalManager.UpdateAsync(jounalDeposit, autoDeposit.DocumentType));
                

            }

            entity.UpdateStatusToVoid();
            CheckErrors(await _bankTransferManager.UpdateAsync(entity));
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Banks_Transfers_UpdateStatusToDraft)]
        public async Task<NullableIdDto<Guid>> UpdateStatusToDraft(BankStatus input)
        {
            var @entity = await _bankTransferManager.GetAsync(input.Id, true);
            
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            entity.UpdateStatusToDraft();
            CheckErrors(await _bankTransferManager.UpdateAsync(entity));

            //delete deposit 
            var itemDepositId = _depositRepository.GetAll()
                   .Where(t => t.BankTransferId == input.Id).Select(t => t.Id).FirstOrDefault();
            var inputDeleteDeposit = new CarlEntityDto() { IsConfirm = input.IsConfirm,Id = itemDepositId };
            await DeleteDeposit(inputDeleteDeposit);

            //delete withdraw
            var itemWithdrawId = _withdrawRepository.GetAll()
                .Where(t => t.BankTransferId == input.Id).Select(t => t.Id).FirstOrDefault();
            var inputDeleteWithdraw = new CarlEntityDto() {IsConfirm = input.IsConfirm , Id = itemWithdrawId };
            await DeleteWithdraw(inputDeleteWithdraw);

            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        private async Task DeleteDeposit(CarlEntityDto input)
        {
            var @jounal = await _journalRepository.GetAll()
                   .Include(u => u.Deposit)
                   .Where(u => u.JournalType == JournalType.Deposit && u.DepositId == input.Id)
                   .FirstOrDefaultAsync();

            //query get Deposit
            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                  .Where(t => (t.LockKey == TransactionLockType.BankTransaction)
                  && t.IsLock == true && t.LockDate.Value.Date >= jounal.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var @entity = @jounal.Deposit;

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            //query get journal and delete
            @jounal.UpdateDeposit(null);

            //query get journal item and delete
            var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == jounal.Id).ToListAsync();
            foreach (var ji in jounalItems)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(ji));
            }
            CheckErrors(await _journalManager.RemoveAsync(@jounal));

            //query get deposit item and delete 
            var depositItems = await _depositItemRepository.GetAll()
                .Where(u => u.DepositId == entity.Id).ToListAsync();

            foreach (var di in depositItems)
            {
                CheckErrors(await _depositItemManager.RemoveAsync(di));
            }
            CheckErrors(await _depositManager.RemoveAsync(entity));
        }

        private async Task DeleteWithdraw(CarlEntityDto input)
        {
            var @jounal = await _journalRepository.GetAll()
                .Include(u => u.withdraw)
                .Where(u => u.JournalType == JournalType.Withdraw && u.WithdrawId == input.Id)
                .FirstOrDefaultAsync();
            //query get withdraw
            var @entity = @jounal.withdraw;

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            if (input.IsConfirm == false)
            {
                var locktransaction = await _lockRepository.GetAll()
                  .Where(t => (t.LockKey == TransactionLockType.BankTransaction)
                  && t.IsLock == true && t.LockDate != null && t.LockDate.Value.Date >= jounal.Date.Date).AsNoTracking().CountAsync();

                if (locktransaction > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            @jounal.UpdateWithdraw(null);

            //query get journal item and delete
            var @jounalItems = await _journalItemRepository.GetAll().Where(u => u.JournalId == jounal.Id).ToListAsync();
            foreach (var ji in jounalItems)
            {
                CheckErrors(await _journalItemManager.RemoveAsync(ji));
            }
            CheckErrors(await _journalManager.RemoveAsync(@jounal));

            //query get withdraw item and delete 
            var withdrawItems = await _withdrawItemRepository.GetAll()
                .Where(u => u.WithdrawId == entity.Id).ToListAsync();

            foreach (var bi in withdrawItems)
            {

                CheckErrors(await _withdrawItemManager.RemoveAsync(bi));

            }

            CheckErrors(await _withdrawManager.RemoveAsync(entity));
        }
        
        private async Task  UpdateDeposit(UpdateDepositInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // update Journal 
            var @journal = await _journalRepository
                              .GetAll()
                              .Where(u => u.JournalType == JournalType.Deposit && u.DepositId == input.Id)
                              .FirstOrDefaultAsync();

            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                      .Where(t => t.IsLock == true && t.LockDate != null &&
                                      (t.LockDate.Value.Date >= journal.Date.Date || t.LockDate.Value.Date >= input.Date.Date)
                                      && t.LockKey == TransactionLockType.BankTransaction).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }

            var autoDeposit = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Deposit);
            if (autoDeposit.CustomFormat == true)
            {
                input.DepositNo = journal.JournalNo;
            }
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
            deposit.Update(tenantId, input.ReceiveFromVendorId, input.ReceiveFromCustomerId, input.Total);


            CheckErrors(await _journalManager.UpdateAsync(@journal, autoDeposit.DocumentType));

            CheckErrors(await _journalItemManager.UpdateAsync(accountItem));
            CheckErrors(await _depositManager.UpdateAsync(deposit, false));


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
            await CurrentUnitOfWork.SaveChangesAsync();
           
        }

        public async Task UpdateWithdraw(UpdateWithdrawInput input)
        {
            //validate withdrawItem when create by none
            if (input.WithdrawItems.Count == 0)
            {
                throw new UserFriendlyException(L("ItemInfoHaveAtLeastOneRecord"));
            }
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            // update Journal 
            var @journal = await _journalRepository
                              .GetAll()
                              .Where(u => u.JournalType == JournalType.Withdraw && u.WithdrawId == input.Id)
                              .FirstOrDefaultAsync();

            if (input.IsConfirm == false)
            {
                var validateLockDate = await _lockRepository.GetAll()
                                     .Where(t => t.IsLock == true && t.LockDate != null &&
                                     (t.LockDate.Value.Date >= journal.Date.Date || t.LockDate.Value.Date >= input.Date.Date)
                                     && t.LockKey == TransactionLockType.BankTransaction).CountAsync();

                if (validateLockDate > 0)
                {
                    throw new UserFriendlyException(L("ThisRecordLockAreadyCanNotEdit"));
                }
            }
            var autoWithdraw = await _autoSequenceManager.GetAutoSequenceAsync(DocumentType.Withdraw);
            if (autoWithdraw.CustomFormat == true)
            {
                input.WithdrawNo = journal.JournalNo;
            }
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

            CheckErrors(await _journalManager.UpdateAsync(@journal, autoWithdraw.DocumentType));

            CheckErrors(await _journalItemManager.UpdateAsync(bankAccountItem));
            CheckErrors(await _withdrawManager.UpdateAsync(withdraw, false));


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
            await CurrentUnitOfWork.SaveChangesAsync();
           
        }

    }
}
