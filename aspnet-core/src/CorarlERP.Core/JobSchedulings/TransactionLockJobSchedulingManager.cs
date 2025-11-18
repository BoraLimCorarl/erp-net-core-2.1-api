using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Timing;
using CorarlERP.Locks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.JobSchedulings
{
    public class TransactionLockJobSchedulingManager : CorarlERPDomainServiceBase, ITransactionLockJobSchedulingManager
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICorarlRepository<Lock, long> _lockRepository;
        private readonly ILockManager _lockManager;
        private readonly ICorarlRepository<TransactionLockSheduleItem, long> _transactionLockScheduleItemRepository;

        public TransactionLockJobSchedulingManager(
            IUnitOfWorkManager unitOfWorkManager,
            ICorarlRepository<Lock, long> lockRepository,
            ILockManager lockManager,
            ICorarlRepository<TransactionLockSheduleItem, long> transactionLockScheduleItemRepository
            )
        {
            _unitOfWorkManager = unitOfWorkManager;
            _lockRepository = lockRepository;
            _lockManager = lockManager;
            _transactionLockScheduleItemRepository = transactionLockScheduleItemRepository;
        }
        public async Task ScheduleLock(int tenantId,
                                       long userId,
                                       long transactionLockId)
        {
            await AutoLock(tenantId, userId, transactionLockId);
        }




        private async Task AutoLock(int tenantId, long userId, long transactionLockId)
        {

            var scheduleItems = new List<TransactionLockSheduleItem>();
            var lockQuery = new List<Lock>();
            var schedule = new TransactionLockSchedule();
            var lsLockCreate = new List<Lock>();
            var lsLockUpdate = new List<Lock>();
            TimeZoneInfo tz = base.MyTimeZoneInfo();
            var today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {              
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        scheduleItems = await _transactionLockScheduleItemRepository.GetAll()
                               .Include(s => s.TransactionLockShedule)
                               .AsNoTracking()
                               .Where(s => s.TransactionLockSheduleId == transactionLockId).ToListAsync();
                        if (!scheduleItems.Any()) return;
                        schedule = scheduleItems.FirstOrDefault().TransactionLockShedule;
                        lockQuery = await _lockRepository.GetAll().AsNoTracking().ToListAsync();
                    }               
            }

            if (lockQuery.Count() > 0)
            {
                foreach (var l in lockQuery)
                {
                    var item = scheduleItems.FirstOrDefault(s => s.LockTransaction == l.LockKey);
                    if (item != null)
                    {
                        l.SetLockDate(schedule.ScheduleDate == ScheduleDate.Today ? today :
                                      schedule.ScheduleDate == ScheduleDate.Yesterday ? today.AddDays(-1) :
                                      today.AddDays(-schedule.DaysBeforeYesterday - 1));
                        l.SetIsLock(true);
                        lsLockUpdate.Add(l);
                    }
                }
            }
            else
            {
                var createLocks = new List<dynamic>()
                {
                    new { IsLock = false, LockDate= today, TransactionLockType = TransactionLockType.PurchaseOrder, TransactionLockTypeName = TransactionLockType.PurchaseOrder.ToString() },
                    new { IsLock = false, LockDate= today, TransactionLockType = TransactionLockType.ItemReceipt, TransactionLockTypeName = TransactionLockType.ItemReceipt.ToString()},
                    new { IsLock = false, LockDate= today, TransactionLockType = TransactionLockType.Bill, TransactionLockTypeName = TransactionLockType.Bill.ToString()},
                    new { IsLock = false, LockDate= today, TransactionLockType = TransactionLockType.PayBill, TransactionLockTypeName = TransactionLockType.PayBill.ToString() },
                    new { IsLock = false, LockDate= today, TransactionLockType = TransactionLockType.SaleOrder, TransactionLockTypeName = TransactionLockType.SaleOrder.ToString() },
                    new { IsLock = false, LockDate= today, TransactionLockType = TransactionLockType.ItemIssue, TransactionLockTypeName = TransactionLockType.ItemIssue.ToString() },
                    new { IsLock = false, LockDate= today, TransactionLockType = TransactionLockType.Invoice, TransactionLockTypeName = TransactionLockType.Invoice.ToString() },
                    new { IsLock = false, LockDate= today, TransactionLockType = TransactionLockType.ReceivePayment, TransactionLockTypeName = TransactionLockType.ReceivePayment.ToString() },
                    new { IsLock = false, LockDate= today, TransactionLockType = TransactionLockType.ProductionOrder, TransactionLockTypeName = TransactionLockType.ProductionOrder.ToString() },
                    new { IsLock = false, LockDate= today, TransactionLockType = TransactionLockType.InventoryTransaction, TransactionLockTypeName = TransactionLockType.InventoryTransaction.ToString() },
                    new { IsLock = false, LockDate= today, TransactionLockType = TransactionLockType.TransferOrder, TransactionLockTypeName = TransactionLockType.TransferOrder.ToString() },
                    new { IsLock = false, LockDate= today, TransactionLockType = TransactionLockType.PhysicalCount, TransactionLockTypeName = TransactionLockType.PhysicalCount.ToString() },
                    new { IsLock = false, LockDate= today, TransactionLockType = TransactionLockType.BankTransaction, TransactionLockTypeName = TransactionLockType.BankTransaction.ToString() },
                    new { IsLock = false, LockDate= today, TransactionLockType = TransactionLockType.BankTransfer, TransactionLockTypeName = TransactionLockType.BankTransfer.ToString() },
                    new { IsLock = false, LockDate= today, TransactionLockType = TransactionLockType.GeneralJournal, TransactionLockTypeName = TransactionLockType.GeneralJournal.ToString() },
                }.ToList();

                foreach (var i in createLocks)
                {
                    //var l = Lock.Create(tenantId, userId, i.IsLock, i.LockDate, i.TransactionLockType, i.GenerateDate, i.Password, i.ExpiredTime);
                    var l = Lock.Create(tenantId, userId, i.IsLock, i.LockDate, i.TransactionLockType, null, "", 0);
                    var item = scheduleItems.FirstOrDefault(s => s.LockTransaction == l.LockKey);
                    if (item != null)
                    {
                        l.SetLockDate(schedule.ScheduleDate == ScheduleDate.Today ? today :
                                     schedule.ScheduleDate == ScheduleDate.Yesterday ? today.AddDays(-1) :
                                     today.AddDays(-schedule.DaysBeforeYesterday - 1));
                        l.SetIsLock(true);
                    }

                    lsLockCreate.Add(l);
                }
            }

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    if (lsLockCreate != null && lsLockCreate.Count > 0) await _lockRepository.BulkInsertAsync(lsLockCreate);
                    if (lsLockUpdate != null && lsLockUpdate.Count > 0) await _lockRepository.BulkUpdateAsync(lsLockUpdate);
                }
                await uow.CompleteAsync();
            }
        }

    }
}
