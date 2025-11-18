using Abp.Domain.Services;
using CorarlERP.Locks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.JobSchedulings
{
    public interface ITransactionLockJobSchedulingManager: IDomainService
    {
        Task ScheduleLock(int tenantId,
                                       long userId,
                                       long transactionLockId);
    }
}
