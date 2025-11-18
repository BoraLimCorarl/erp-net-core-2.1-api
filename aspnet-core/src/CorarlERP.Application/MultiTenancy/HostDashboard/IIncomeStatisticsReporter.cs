using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CorarlERP.MultiTenancy.HostDashboard.Dto;

namespace CorarlERP.MultiTenancy.HostDashboard
{
    public interface IIncomeStatisticsService
    {
        Task<List<IncomeStastistic>> GetIncomeStatisticsData(DateTime startDate, DateTime endDate,
            ChartDateInterval dateInterval);
    }
}