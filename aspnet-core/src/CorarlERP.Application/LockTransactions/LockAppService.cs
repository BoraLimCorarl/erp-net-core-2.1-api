using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using CorarlERP.Authorization;
using CorarlERP.Locks;
using CorarlERP.LockTransactions.Dto;
using CorarlERP.TransactionLockSchedules.Dto;
using Microsoft.EntityFrameworkCore;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.LockTransactions
{
    [AbpAuthorize]
    public class LockAppService : CorarlERPAppServiceBase, ILockAppService
    {
        private readonly ILockManager _lockManager;
        private readonly IRepository<Lock, long> _lockRepository;
        private readonly IRepository<PermissionLock, long> _permissionLockRepository;

        public LockAppService(
            ILockManager lockManager, 
            IRepository<Lock, long> lockRepository,
            IRepository<PermissionLock, long> permissionLockRepository)
        {
            _lockManager = lockManager;
            _lockRepository = lockRepository;
            _permissionLockRepository = permissionLockRepository;
        }

        [DisableAuditing]
        public PagedResultDto<LockTransactionActionOutput> FindLockActions(string filter)
        {

            var querys = new List<LockTransactionActionOutput> {
                new LockTransactionActionOutput{ Id = Convert.ToInt32(LockAction.Create), Key = LockAction.Create.ToString(), Value = L(LockAction.Create.ToString())},
                new LockTransactionActionOutput{ Id = Convert.ToInt32(LockAction.Update), Key = LockAction.Update.ToString() , Value = L(LockAction.Update.ToString())},
                new LockTransactionActionOutput{ Id = Convert.ToInt32(LockAction.Delete), Key = LockAction.Delete.ToString(), Value = L(LockAction.Delete.ToString())},
            }.Where(s => filter.IsNullOrWhiteSpace() || s.Value.Contains(filter) ).ToList();



            var resultCount = querys.Count;
            return new PagedResultDto<LockTransactionActionOutput>(resultCount, querys);
        }

        [DisableAuditing]
        public PagedResultDto<LockTransactionActionOutput> FindLockTransactions(string filter)
        {
            var querys = new List<LockTransactionActionOutput> {
                new LockTransactionActionOutput{ Id = Convert.ToInt32(TransactionLockType.Bill), Key = TransactionLockType.Bill.ToString(), Value = L(TransactionLockType.Bill.ToString())},
                new LockTransactionActionOutput{ Id = Convert.ToInt32(TransactionLockType.ItemReceipt), Key = TransactionLockType.ItemReceipt.ToString(), Value = L(TransactionLockType.ItemReceipt.ToString())},
                new LockTransactionActionOutput{ Id = Convert.ToInt32(TransactionLockType.PayBill), Key = TransactionLockType.PayBill.ToString(), Value = L(TransactionLockType.PayBill.ToString())},
                new LockTransactionActionOutput{ Id = Convert.ToInt32(TransactionLockType.PurchaseOrder), Key = TransactionLockType.PurchaseOrder.ToString() , Value = L(TransactionLockType.PurchaseOrder.ToString())},
                new LockTransactionActionOutput{ Id = Convert.ToInt32(TransactionLockType.Invoice), Key = TransactionLockType.Invoice.ToString(), Value = L(TransactionLockType.Invoice.ToString())},
                new LockTransactionActionOutput{ Id = Convert.ToInt32(TransactionLockType.ItemIssue), Key = TransactionLockType.ItemIssue.ToString(), Value = L(TransactionLockType.ItemIssue.ToString())},
                new LockTransactionActionOutput{ Id = Convert.ToInt32(TransactionLockType.ReceivePayment), Key = TransactionLockType.ReceivePayment.ToString(), Value = L(TransactionLockType.ReceivePayment.ToString())},
                new LockTransactionActionOutput{ Id = Convert.ToInt32(TransactionLockType.SaleOrder), Key = TransactionLockType.SaleOrder.ToString(), Value = L(TransactionLockType.SaleOrder.ToString())},
                new LockTransactionActionOutput{ Id = Convert.ToInt32(TransactionLockType.InventoryTransaction), Key = TransactionLockType.InventoryTransaction.ToString(), Value = L(TransactionLockType.InventoryTransaction.ToString())},
                new LockTransactionActionOutput{ Id = Convert.ToInt32(TransactionLockType.ProductionOrder), Key = TransactionLockType.ProductionOrder.ToString(), Value = L(TransactionLockType.ProductionOrder.ToString())},
                new LockTransactionActionOutput{ Id = Convert.ToInt32(TransactionLockType.TransferOrder), Key = TransactionLockType.TransferOrder.ToString(), Value = L(TransactionLockType.TransferOrder.ToString())},
                new LockTransactionActionOutput{ Id = Convert.ToInt32(TransactionLockType.PhysicalCount), Key = TransactionLockType.PhysicalCount.ToString(), Value = L("PhysicalCount")},
                new LockTransactionActionOutput{ Id = Convert.ToInt32(TransactionLockType.BankTransaction), Key = TransactionLockType.BankTransaction.ToString(), Value = L(TransactionLockType.BankTransaction.ToString())},
                new LockTransactionActionOutput{ Id = Convert.ToInt32(TransactionLockType.BankTransfer), Key = TransactionLockType.BankTransfer.ToString(), Value = L(TransactionLockType.BankTransfer.ToString())},
                new LockTransactionActionOutput{ Id = Convert.ToInt32(TransactionLockType.GeneralJournal), Key = TransactionLockType.GeneralJournal.ToString(), Value = L(TransactionLockType.GeneralJournal.ToString())},
                new LockTransactionActionOutput{ Id = Convert.ToInt32(TransactionLockType.DeliverySchedule), Key = TransactionLockType.DeliverySchedule.ToString(), Value = L(TransactionLockType.DeliverySchedule.ToString())},
            }.Where(s => filter.IsNullOrWhiteSpace() || s.Value.Contains(filter)).ToList();

            var resultCount = querys.Count;
            return new PagedResultDto<LockTransactionActionOutput>(resultCount, querys);
        }

        

        [DisableAuditing]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_GetList)]
        public async Task<PagedResultDto<GetListLockOutput>> GetList(GetListLockInput input)
        {
            return await GetListHelper(input);
        }

        private async Task<PagedResultDto<GetListLockOutput>> GetListHelper(GetListLockInput input)
        {
            var query = _lockRepository
                           .GetAll().Select(t => new GetListLockOutput
                           {
                               Id = t.Id,
                               TransactionLockTypeName = t.LockKey.ToString(),
                               TransactionLockType = t.LockKey,
                               IsLock = t.IsLock,
                               LockDate = t.LockDate,
                               Password = t.Password,
                               GenerateDate = t.GenerateDate,
                               ExpiredTime = t.ExpiredTime,
                           }).AsNoTracking();
            var @entities = await query.ToListAsync();
            if (query.Count() > 0)
            {
                var resultCount = await query.CountAsync();
                return new PagedResultDto<GetListLockOutput>(resultCount, ObjectMapper.Map<List<GetListLockOutput>>(@entities));

            }
            else
            {
                var entities1 = new List<GetListLockOutput>()
                {
                    new GetListLockOutput {IsLock = false,LockDate= DateTime.Now,TransactionLockType = TransactionLockType.PurchaseOrder,TransactionLockTypeName = TransactionLockType.PurchaseOrder.ToString() },
                    new GetListLockOutput {IsLock = false,LockDate= DateTime.Now,TransactionLockType = TransactionLockType.ItemReceipt,TransactionLockTypeName = TransactionLockType.ItemReceipt.ToString()},
                    new GetListLockOutput {IsLock = false,LockDate= DateTime.Now,TransactionLockType = TransactionLockType.Bill,TransactionLockTypeName = TransactionLockType.Bill.ToString()},
                    new GetListLockOutput {IsLock = false,LockDate= DateTime.Now,TransactionLockType = TransactionLockType.PayBill,TransactionLockTypeName = TransactionLockType.PayBill.ToString() },
                    new GetListLockOutput {IsLock = false,LockDate= DateTime.Now,TransactionLockType = TransactionLockType.SaleOrder,TransactionLockTypeName = TransactionLockType.SaleOrder.ToString() },
                    new GetListLockOutput {IsLock = false,LockDate= DateTime.Now,TransactionLockType = TransactionLockType.ItemIssue,TransactionLockTypeName = TransactionLockType.ItemIssue.ToString() },
                    new GetListLockOutput {IsLock = false,LockDate= DateTime.Now,TransactionLockType = TransactionLockType.Invoice,TransactionLockTypeName = TransactionLockType.Invoice.ToString() },
                    new GetListLockOutput {IsLock = false,LockDate= DateTime.Now,TransactionLockType = TransactionLockType.ReceivePayment,TransactionLockTypeName = TransactionLockType.ReceivePayment.ToString() },
                    new GetListLockOutput {IsLock = false,LockDate= DateTime.Now,TransactionLockType = TransactionLockType.ProductionOrder,TransactionLockTypeName = TransactionLockType.ProductionOrder.ToString() },
                    new GetListLockOutput {IsLock = false,LockDate= DateTime.Now,TransactionLockType = TransactionLockType.InventoryTransaction,TransactionLockTypeName = TransactionLockType.InventoryTransaction.ToString() },
                    new GetListLockOutput {IsLock = false,LockDate= DateTime.Now,TransactionLockType = TransactionLockType.TransferOrder,TransactionLockTypeName = TransactionLockType.TransferOrder.ToString() },
                    new GetListLockOutput {IsLock = false,LockDate= DateTime.Now,TransactionLockType = TransactionLockType.PhysicalCount,TransactionLockTypeName = TransactionLockType.PhysicalCount.ToString() },
                    new GetListLockOutput {IsLock = false,LockDate= DateTime.Now,TransactionLockType = TransactionLockType.BankTransaction,TransactionLockTypeName = TransactionLockType.BankTransaction.ToString() },
                    new GetListLockOutput {IsLock = false,LockDate= DateTime.Now,TransactionLockType = TransactionLockType.BankTransfer,TransactionLockTypeName = TransactionLockType.BankTransfer.ToString() },
                    new GetListLockOutput {IsLock = false,LockDate= DateTime.Now,TransactionLockType = TransactionLockType.GeneralJournal,TransactionLockTypeName = TransactionLockType.GeneralJournal.ToString() },
                    new GetListLockOutput {IsLock = false,LockDate= DateTime.Now,TransactionLockType = TransactionLockType.DeliverySchedule,TransactionLockTypeName = TransactionLockType.DeliverySchedule.ToString() },
                }.ToList();
                var resultCount = entities1.Count();
                return new PagedResultDto<GetListLockOutput>(resultCount, ObjectMapper.Map<List<GetListLockOutput>>(@entities1));
            }
        }

        [DisableAuditing]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_CreateOrUpdate)]
        public async  Task<NullableIdDto<long>> CreateOrUpdate(UpdateLockInput input)
        {
            return await CreateOrUpdateHelper(input);
        }

        private async Task<NullableIdDto<long>> CreateOrUpdateHelper(UpdateLockInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            foreach (var i in input.LockItems)
            {
                if (i.Id > 0)
                {
                    var @entity = await _lockRepository.GetAll().Where(t => t.Id == i.Id).FirstOrDefaultAsync();
                    entity.Update(userId, i.IsLock, i.LockDate, i.TransactionLockType, i.GenerateDate, i.Password, i.ExpiredTime);

                    await _lockManager.UpdateAsync(entity);
                }
                else
                {
                    var @entity = Lock.Create(tenantId, userId, i.IsLock, i.LockDate, i.TransactionLockType, i.GenerateDate, i.Password, i.ExpiredTime);

                    await _lockManager.CreateAsync(entity);
                }

            }
            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<long>() { Id = input.Id };
        }

        [DisableAuditing]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_GeneratePasswork)]
        public string GenenratePasswork()
        {
           var  password = Guid.NewGuid().ToString("N").Truncate(10).ToUpperInvariant();

                return  password;

        }

        [DisableAuditing]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_Find)]
        public async Task<PagedResultDto<GetListLockOutput>> Find(GetListFindLockInput input)
        {
            //var query = _lockRepository
            //              .GetAll()
            //              .Where(t=> input.TransactionLockType.Any(g=>g == t.LockKey) && t.IsLock && t.Password != null) 
            //              .Select(t => new GetListLockOutput
            //              {
            //                  Id = t.Id,
            //                  TransactionLockTypeName = t.LockKey.ToString(),
            //                  TransactionLockType = t.LockKey,
            //                  IsLock = t.IsLock,
            //                  LockDate = t.LockDate,
            //                  Password = t.Password,
            //                  GenerateDate = t.GenerateDate,
            //                  ExpiredTime = t.ExpiredTime,
            //              }).AsNoTracking();

            //var @entities = await query.ToListAsync();
            //var resultCount = await query.CountAsync();
            //return new PagedResultDto<GetListLockOutput>(resultCount, ObjectMapper.Map<List<GetListLockOutput>>(@entities));


            var query = from l in _lockRepository
                          .GetAll()
                          .Where(t => input.TransactionLockType.Any(g => g == t.LockKey) && t.IsLock)
                          .AsNoTracking()
                        join p in _permissionLockRepository.GetAll().Where(s => s.IsActive).AsNoTracking()
                        on l.LockKey equals p.LockTransaction
                        into pls
                        where pls.Any()

                        select new GetListLockOutput
                        {
                              Id = l.Id,
                              TransactionLockTypeName = l.LockKey.ToString(),
                              TransactionLockType = l.LockKey,
                              IsLock = l.IsLock,
                              LockDate = l.LockDate,
                              Password = l.Password,
                              GenerateDate = l.GenerateDate,
                              ExpiredTime = l.ExpiredTime,
                              PermissionLocks = pls.Select(s => new PermissionLocks.Dto.PermissionLockDetailOutput { 
                                Id = s.Id,
                                ExpiredDuration = s.ExpiredDuration,
                                IsActive = s.IsActive,
                                LocationId = s.LocationId,
                                LockAction = s.LockAction,
                                PermissionCode = s.PermissionCode,
                                TransactionNo = s.TransactionNo,
                                PermissionDate = s.PermissionDate,
                                LockTransaction = s.LockTransaction,          
                                PermissionCodeGenerateDate = s.PermissionCodeGenerateDate,
                              }).ToList(),
                        };


            var @entities = await query.ToListAsync();          
                var resultCount = await query.CountAsync();
                return new PagedResultDto<GetListLockOutput>(resultCount, ObjectMapper.Map<List<GetListLockOutput>>(@entities));

            
        }

        [DisableAuditing]
        public async Task AutoLock(TransactionLockScheduleDetailOutput input)
        {
            if (input.TransactionLockScheduleItems == null || !input.TransactionLockScheduleItems.Any()) return;

            var getListInput = new GetListLockInput();            
            var result = await GetListHelper(getListInput);
            var locks = result.Items;

            var lockInput = new UpdateLockInput();
            lockInput.LockItems = new List<CreateLockInput>();

            foreach (var l in locks)
            {
                var item = new CreateLockInput() {
                    Id = l.Id,
                    IsLock = l.IsLock,
                    LockDate = l.LockDate,                   
                    TransactionLockType = l.TransactionLockType,
                    ExpiredTime = l.ExpiredTime,
                    GenerateDate = l.GenerateDate,
                    Password = l.Password,
                };

                var t = input.TransactionLockScheduleItems.FirstOrDefault(s => s.TransactionLockType == l.TransactionLockType);
                if(t != null)
                {
                    item.LockDate = input.ScheduleDate == ScheduleDate.Today ? Clock.Now : Clock.Now.AddDays(-1);                    
                    item.IsLock = true;
                }

                lockInput.LockItems.Add(item);
            }

            await CreateOrUpdateHelper(lockInput);
        }

        [DisableAuditing]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_CreateOrUpdate)]
        public async Task RemoveDuplicateLock()
        {
            var locks = await _lockRepository.GetAll()                       
                        .GroupBy(s => s.LockKey)
                        .Where(s => s.Count() > 1)
                        .Select(s => s.FirstOrDefault())
                        .ToListAsync();

            if (locks.Any())
            {
                foreach(var l in locks)
                {
                    await _lockRepository.DeleteAsync(l);
                }
            }

        }

        [DisableAuditing]
        [AbpAuthorize(AppPermissions.Pages_Tenant_Accounting_Locks_CreateOrUpdate)]
        public async Task ClearLock()
        {
            var locks = await _lockRepository.GetAll()
                        .ToListAsync();

            if (locks.Any())
            {
                foreach (var l in locks)
                {
                    await _lockRepository.DeleteAsync(l);
                }
            }
        }
    }
}
