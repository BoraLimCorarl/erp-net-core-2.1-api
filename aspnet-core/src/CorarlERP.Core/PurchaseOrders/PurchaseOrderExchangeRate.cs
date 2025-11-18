using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using CorarlERP.Currencies;
using Abp.Timing;

namespace CorarlERP.PurchaseOrders
{
    [Table("CarlPurchaseOrderExchangeRates")]
    public class PurchaseOrderExchangeRate : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public Guid PurchaseOrderId { get; private set; }
        public PurchaseOrder PurchaseOrder { get; private set; }
        public long FromCurrencyId { get; private set; }
        public Currency FromCurrency { get; private set; }
        public long ToCurrencyId { get; private set; }
        public Currency ToCurrency { get; private set; }
        public decimal Bid { get; private set; }
        public decimal Ask { get; private set; }

        public static PurchaseOrderExchangeRate Create(int tenantId, long userId, Guid purchaseOrderId, long fromCurrencyId, long toCurrencyId, decimal bid, decimal ask)
        {
            return new PurchaseOrderExchangeRate
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                PurchaseOrderId = purchaseOrderId,
                FromCurrencyId = fromCurrencyId,
                ToCurrencyId = toCurrencyId,
                Bid = bid,
                Ask = ask
            };
        }

        public void Update(long userId, Guid purchaseOrderId, long fromCurrencyId, long toCurrencyId, decimal bid, decimal ask)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            PurchaseOrderId = purchaseOrderId;
            FromCurrencyId = fromCurrencyId;
            ToCurrencyId = toCurrencyId;
            Bid = bid;
            Ask = ask;
        }
    }
}
