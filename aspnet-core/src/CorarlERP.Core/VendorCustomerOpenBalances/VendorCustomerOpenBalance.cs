using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.AccountCycles;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.VendorCustomerOpenBalances
{
    [Table("CarlErpVendorOpenBalances")]
    public class VendorOpenBalance : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public Guid TransactionId { get; private set; }

        public JournalType Key { get; private set; }

        public long AccountCycleId { get; private set; }
        public AccountCycle AccountCycle { get; private set; }

        public decimal Balance { get; private set; }
        public decimal MuliCurrencyBalance { get; private set; }

        public long LocationId { get; private set; }
        public DateTime Date { get; private set; }

        public static VendorOpenBalance Create(int? tenantId, long creatorUserId,Guid transactionId,JournalType key,long accountCycleId, decimal balance, decimal multiCurrencyBalance, long locationId, DateTime date)
        {
            return new VendorOpenBalance()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,              
                AccountCycleId = accountCycleId,
                Key = key,
                TransactionId = transactionId,
                Balance = balance,
                MuliCurrencyBalance = multiCurrencyBalance,
                LocationId = locationId,
                Date = date,
            };
        }
        
        public void Update(long lastModifiedUserId, Guid transactionId, JournalType key, long accountCycleId, decimal balance, decimal multiCurrencyBalance, long locationId, DateTime date)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            AccountCycleId = accountCycleId;
            Key = key;
            TransactionId = transactionId;
            Balance = balance;
            MuliCurrencyBalance = multiCurrencyBalance;
            LocationId = locationId;
            Date = date;
        } 
    }

    [Table("CarlErpCustomerOpenBalances")]
    public class CustomerOpenBalance : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public Guid TransactionId { get; private set; }

        public JournalType Key { get; private set; }

        public long AccountCycleId { get; private set; }
        public AccountCycle AccountCycle { get; private set; }

        public decimal Balance { get; private set; }
        public decimal MuliCurrencyBalance { get; private set; }

        public long LocationId { get; private set; }
        public DateTime Date { get; private set; }

        public static CustomerOpenBalance Create(int? tenantId, long creatorUserId, Guid transactionId, JournalType key, long accountCycleId, decimal balance, decimal multiCurrencyBalance, long locationId, DateTime date)
        {
            return new CustomerOpenBalance()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                AccountCycleId = accountCycleId,
                Key = key,
                TransactionId = transactionId,
                Balance = balance,
                MuliCurrencyBalance = multiCurrencyBalance,
                LocationId = locationId,
                Date = date,
            };
        }

        public void Update(long lastModifiedUserId, Guid transactionId, JournalType key, long accountCycleId, decimal balance, decimal multiCurrencyBalance, long locationId, DateTime date)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            AccountCycleId = accountCycleId;
            Key = key;
            TransactionId = transactionId;
            Balance = balance;
            MuliCurrencyBalance = multiCurrencyBalance;
            LocationId = locationId;
            Date = date;
        }
    }
}
