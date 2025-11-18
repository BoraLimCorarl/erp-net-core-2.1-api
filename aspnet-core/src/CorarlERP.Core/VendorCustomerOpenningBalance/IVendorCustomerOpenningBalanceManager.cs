using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CorarlERP.VendorCustomerOpenningBalance
{
    public  interface IVendorOpenningBalanceManager : IDomainService
    {
        IQueryable<TransactionOpenningBalanceOutput> GetOpenTransactionQuery(DateTime? fromDate,DateTime toDate,List<long> locations = null);
    }

    public interface ICustomerOpenningBalanceManager : IDomainService
    {
        IQueryable<TransactionOpenningBalanceOutput> GetOpenTransactionQuery(DateTime? fromDate, DateTime toDate, List<long> locations = null);
    }
}
