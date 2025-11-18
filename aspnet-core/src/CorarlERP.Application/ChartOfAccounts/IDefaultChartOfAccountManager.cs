using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ChartOfAccounts
{
    public interface IDefaultChartOfAccountManager: IDomainService
    {
        Task CreateDefaultChartAccounts(int tenantId);
    }
}
