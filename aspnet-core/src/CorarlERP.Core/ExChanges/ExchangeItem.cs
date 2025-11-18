using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Currencies;

namespace CorarlERP.ExChanges
{
    [Table("CarlErpExchangeItems")]
    public class ExchangeItem : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public Currency FromCurrency { get; private set; }
        public long FromCurrencyId { get; private set; }

        public Currency ToCurrency { get; private set; }
        public long ToCurencyId { get; private set; }

        public decimal Bid { get; private set; }

        public decimal Ask { get; private set; }

        public Exchange Exchange { get; private set; }
        public Guid ExchangeId { get; private set; }

        public static ExchangeItem Create(int? tenantId, long creatorUserId, long fromCurrencyId, long toCurrencyId, decimal bid ,decimal ask)
        {
            return new ExchangeItem()
            {
                Id = new Guid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                FromCurrencyId = fromCurrencyId,
                ToCurencyId = toCurrencyId,
                Bid = bid,
                Ask = ask,

            };
        }

        public static ExchangeItem Create(int? tenantId, long creatorUserId, long fromCurrencyId, long toCurrencyId, decimal bid, decimal ask, Exchange exchange)
        {
            var result = Create(tenantId, creatorUserId, fromCurrencyId, toCurrencyId, bid,ask);
            result.Exchange = exchange;
            return result;
        }

        public static ExchangeItem Create(
          int? tenantId, long creatorUserId, long fromCurrencyId,long toCurrencyId, decimal bid, decimal ask, Guid exchangeId)
        {
            var result = Create(tenantId, creatorUserId, fromCurrencyId, toCurrencyId, bid,ask,exchangeId);
            result.ExchangeId = exchangeId;
            return result;
        }


        public void Update(long lastModifiedUserId, long fromCurrencyId, long toCurrencyId,decimal bid, decimal ask, Guid exchangeId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            FromCurrencyId = fromCurrencyId;
            ToCurencyId = toCurrencyId;
            Ask = ask;
            Bid = bid;
            ExchangeId = exchangeId;
        }

    }
}
