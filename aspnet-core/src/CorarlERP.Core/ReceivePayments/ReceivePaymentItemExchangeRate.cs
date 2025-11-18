using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using CorarlERP.Currencies;
using Abp.Timing;

namespace CorarlERP.ReceivePayments
{
    [Table("CarlReceivePaymentItemExchangeRates")]
    public class ReceivePaymentItemExchangeRate : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public Guid ReceivePaymentItemId { get; private set; }
        public ReceivePaymentDetail ReceivePaymentItem { get; private set; }
        public long FromCurrencyId { get; private set; }
        public Currency FromCurrency { get; private set; }
        public long ToCurrencyId { get; private set; }
        public Currency ToCurrency { get; private set; }
        public decimal Bid { get; private set; }
        public decimal Ask { get; private set; }

        public static ReceivePaymentItemExchangeRate Create(int tenantId, long userId, Guid billId, long fromCurrencyId, long toCurrencyId, decimal bid, decimal ask)
        {
            return new ReceivePaymentItemExchangeRate
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                ReceivePaymentItemId = billId,
                FromCurrencyId = fromCurrencyId,
                ToCurrencyId = toCurrencyId,
                Bid = bid,
                Ask = ask
            };
        }

        public void Update(long userId, Guid billId, long fromCurrencyId, long toCurrencyId, decimal bid, decimal ask)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            ReceivePaymentItemId = billId;
            FromCurrencyId = fromCurrencyId;
            ToCurrencyId = toCurrencyId;
            Bid = bid;
            Ask = ask;
        }
    }
}
