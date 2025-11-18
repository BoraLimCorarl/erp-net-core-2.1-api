using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using CorarlERP.Currencies;
using Abp.Timing;

namespace CorarlERP.SaleOrders
{
    [Table("CarlSaleOrderExchangeRates")]
    public class SaleOrderExchangeRate : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public Guid SaleOrderId { get; private set; }
        public SaleOrder SaleOrder { get; private set; }
        public long FromCurrencyId { get; private set; }
        public Currency FromCurrency { get; private set; }
        public long ToCurrencyId { get; private set; }
        public Currency ToCurrency { get; private set; }
        public decimal Bid { get; private set; }
        public decimal Ask { get; private set; }

        public static SaleOrderExchangeRate Create(int tenantId, long userId, Guid saleOrderId, long fromCurrencyId, long toCurrencyId, decimal bid, decimal ask)
        {
            return new SaleOrderExchangeRate
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                SaleOrderId = saleOrderId,
                FromCurrencyId = fromCurrencyId,
                ToCurrencyId = toCurrencyId,
                Bid = bid,
                Ask = ask
            };
        }

        public void Update(long userId, Guid saleOrderId, long fromCurrencyId, long toCurrencyId, decimal bid, decimal ask)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            SaleOrderId = saleOrderId;
            FromCurrencyId = fromCurrencyId;
            ToCurrencyId = toCurrencyId;
            Bid = bid;
            Ask = ask;
        }
    }
}
