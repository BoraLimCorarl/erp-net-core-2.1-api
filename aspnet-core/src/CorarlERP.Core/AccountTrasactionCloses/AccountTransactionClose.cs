using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.AccountCycles;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Currencies;
using CorarlERP.Locations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.AccountTrasactionCloses
{
    [Table("CarlErpAccountTransactionCloses")]
    public class AccountTransactionClose : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            private set { _date = value; DateOnly = value.Date; }
        }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime DateOnly { get; private set; }

        public Guid AccountId { get; private set; }
        public ChartOfAccount Account { get; private set; }

        public decimal Debit { get; private set; }

        public decimal Credit { get; private set;}

        public decimal Balance { get; private set;}

        public AccountCycle AccountCycle { get; set; }
        public long AccountCycleId { get; set; }

        public long? LocationId { get; private set; }
        public Location Location { get; private set; }

        public long? CurrencyId { get; private set; }
        public Currency Currency { get; private set; }
        public decimal MultiCurrencyDebit { get; private set; }
        public decimal MultiCurrencyCredit { get; private set; }
        public decimal MultiCurrencyBalance { get; private set; }

        public static AccountTransactionClose Create(int? tenantId, long? creatorUserId,Guid acountId,
            decimal debit,decimal credit,decimal balance,DateTime date,long accountCycleId,long? locationId, long? currencyId, decimal multiCurrencyDebit, decimal multiCurrencyCredit, decimal multiCurrencyBalance)
        {
            return new AccountTransactionClose()
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                TenantId = tenantId,
                AccountId = acountId,
                Debit = debit,
                Credit = credit,
                Balance = balance,
                Date = date,
                AccountCycleId = accountCycleId,
                LocationId = locationId,
                CurrencyId = currencyId,
                MultiCurrencyDebit = multiCurrencyDebit,
                MultiCurrencyCredit = multiCurrencyCredit,
                MultiCurrencyBalance = multiCurrencyBalance,
            };
        }

        public void Update(long lastModifiedUserId , Guid accountId, decimal debit,
            decimal credit, decimal balance,DateTime date,long accountCycleId,long? locationId, long? currencyId, decimal multiCurrencyDebit, decimal multiCurrencyCredit, decimal multiCurrencyBalance)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            Debit = debit;
            Credit = credit;
            Balance = balance;
            AccountId = accountId;
            Date = date;
            AccountCycleId = accountCycleId;
            LocationId = locationId;
            CurrencyId = currencyId;
            MultiCurrencyDebit = multiCurrencyDebit;
            MultiCurrencyCredit = multiCurrencyCredit;
            MultiCurrencyBalance = multiCurrencyBalance;
        }
    }
}
