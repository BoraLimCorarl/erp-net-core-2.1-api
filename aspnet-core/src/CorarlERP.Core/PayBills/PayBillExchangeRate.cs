using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using CorarlERP.Currencies;
using Abp.Timing;

namespace CorarlERP.PayBills
{
    [Table("CarlPayBillExchangeRates")]
    public class PayBillExchangeRate : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public Guid PayBillId { get; private set; }
        public PayBill PayBill { get; private set; }
        public long FromCurrencyId { get; private set; }
        public Currency FromCurrency { get; private set; }
        public long ToCurrencyId { get; private set; }
        public Currency ToCurrency { get; private set; }
        public decimal Bid { get; private set; }
        public decimal Ask { get; private set; }

        public static PayBillExchangeRate Create(int tenantId, long userId, Guid payBillId, long fromCurrencyId, long toCurrencyId, decimal bid, decimal ask)
        {
            return new PayBillExchangeRate
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                PayBillId = payBillId,
                FromCurrencyId = fromCurrencyId,
                ToCurrencyId = toCurrencyId,
                Bid = bid,
                Ask = ask
            };
        }

        public void Update(long userId, Guid payBillId, long fromCurrencyId, long toCurrencyId, decimal bid, decimal ask)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            PayBillId = payBillId;
            FromCurrencyId = fromCurrencyId;
            ToCurrencyId = toCurrencyId;
            Bid = bid;
            Ask = ask;
        }
    }
}
