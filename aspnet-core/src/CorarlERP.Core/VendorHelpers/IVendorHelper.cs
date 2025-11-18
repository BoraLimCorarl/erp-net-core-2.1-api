using Abp.Domain.Services;
using CorarlERP.VendorHelpers.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.VendorHelpers
{
    public interface IVendorHelper : IDomainService
    {
        IQueryable<BillAndCreditVendorOutput> GetBillAndCreditVendorQueryAsyn(
           DateTime? fromDate, DateTime toDate, string filter, List<Guid> vendors,
           List<Guid> accounts, List<JournalType> journalType, List<long> accountType, CurrencyFilter currencyFilter,List<long>locations = null, List<long> vendortypes = null);

        IQueryable<GetVendorByBillReportOutput> GetBillAndCreditVendorBillsWithBalanceQuery(long userId,
                                    DateTime? fromDate, DateTime toDate, string filter, List<Guid> vendors,
                                    List<Guid> accounts, List<JournalType> journalType, List<long> accountType,
                                    List<long?> users, CurrencyFilter currencyFilter,List<long> locations = null,
                                    List<long> vendorTypes = null);
    }
}
