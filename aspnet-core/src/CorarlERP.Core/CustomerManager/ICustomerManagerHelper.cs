using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.CustomerManager
{
    public interface ICustomerManagerHelper : IDomainService
    {
        IQueryable<InvoiceAndCreditCustomerOutput> GetInvoiceAndCreditCustomerQueryAsyn(
            DateTime fromDate, DateTime toDate, string filter, List<Guid> customers, 
            List<Guid> accounts, List<JournalType> journalType, List<long> accountType, 
            CurrencyFilter currencyFilter,List<long>locations = null,List<long> customerTypes = null);


        IQueryable<GetCustomerByInvoiceReportOutput> GetInvoiceAndCreditCustomerWithBalanceQuery(long userId,
            DateTime? fromDate, DateTime toDate, string filter, List<Guid> customers,
            List<Guid> accounts, List<JournalType> journalType, List<long> accountType, 
            List<long?> users, CurrencyFilter currencyFilter,List<long>locations = null, List<long> customerTypes = null);

        IQueryable<GetCustomerByInvoiceWithPaymentReportOutput> GetInvoiceAndCreditBalanceWithPaymentQuery(long userId,
          DateTime fromDate, DateTime toDate, string filter, List<Guid> customers,
          List<Guid> accounts, List<JournalType> journalType, List<long> accountType, 
          List<long?> users, CurrencyFilter currencyFilter, List<long> locations = null, List<long> customerTypes = null);

    }
}
