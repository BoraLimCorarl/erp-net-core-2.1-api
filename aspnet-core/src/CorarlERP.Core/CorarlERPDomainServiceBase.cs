using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Extensions;
using Abp.Timing.Timezone;
using Abp.UI;
using CorarlERP.AccountCycles;
using CorarlERP.Configuration;
using CorarlERP.MultiTenancy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CorarlERP
{
    public abstract class CorarlERPDomainServiceBase : DomainService
    {
        /* Add your common members for all your domain services. */
        private readonly IRepository<AccountCycle, long> _accountCycleRepository;

        protected CorarlERPDomainServiceBase()
        {
            LocalizationSourceName = CorarlERPConsts.LocalizationSourceName;
        }

        protected CorarlERPDomainServiceBase(IRepository<AccountCycle, long> accountCycleRepository)
        {
            LocalizationSourceName = CorarlERPConsts.LocalizationSourceName;
            _accountCycleRepository = accountCycleRepository;
        }
        protected TimeZoneInfo MyTimeZoneInfo()
        {
            var tz = TimezoneHelper.FindTimeZoneInfo("SE Asia Standard Time");
            return tz;

        }



        protected virtual AccountCycle GetPreviousCloseCyleAsync(DateTime date)
        {
            var query = _accountCycleRepository.GetAll().Where(u => u.EndDate != null &&
                                                               u.EndDate.Value.Date <= date.Date).AsNoTracking()
                         .OrderByDescending(u => u.EndDate.Value).FirstOrDefault();

            return query;
        }


        protected virtual AccountCycle GetNextCloseCyleAsync(DateTime date)
        {
            var query = _accountCycleRepository.GetAll().AsNoTracking().Where(u => u.EndDate != null &&
                                                                             u.EndDate.Value.Date >= date.Date)
                         .OrderBy(u => u.EndDate.Value).FirstOrDefault();

            return query;
        }

        protected virtual AccountCycle GetCyleAsync(DateTime date)
        {
            var result = _accountCycleRepository.GetAll().AsNoTracking()
                          .Where(u => u.StartDate.Date <= date.Date && (u.EndDate == null || date.Date <= u.EndDate.Value.Date))
                          .OrderByDescending(u => u.StartDate).FirstOrDefault();

            return result;

        }

        protected virtual IQueryable<AccountCycle> GetCloseCyleQuery(DateTime? fromDate = null)
        {
            var query = _accountCycleRepository
                            .GetAll()
                            .AsNoTracking()
                            .WhereIf(fromDate != null, u => u.StartDate >= fromDate.Value);
            return query;
        }


        protected async Task CheckClosePeriod(DateTime dateTime)
        {
            var @closePeroid = await _accountCycleRepository.GetAll().Where(t => t.EndDate == null).AsNoTracking().FirstOrDefaultAsync();

            if (closePeroid.StartDate.Date > dateTime.Date)
            {
                throw new UserFriendlyException(L("PeriodIsClose"));
            }
        }

        protected string AppName
        {
            get
            {
                var hostingEnvironment = IocManager.Instance.Resolve<IHostingEnvironment>();
                var appConfiguration = hostingEnvironment.GetAppConfiguration();
                return appConfiguration["App:Name"];
            }
        }
    }
}
