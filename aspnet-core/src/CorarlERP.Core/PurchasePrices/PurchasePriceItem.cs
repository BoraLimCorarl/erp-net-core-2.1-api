using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Currencies;
using CorarlERP.Items;
using CorarlERP.Vendors;

namespace CorarlERP.PurchasePrices
{

    [Table("CarlErpPurchasePriceItems")]
    public class PurchasePriceItem : AuditedEntity<Guid>, IMayHaveTenant
    {
       
        public long CurrencyId { get; private set; }
        public Currency Currency { get; private set; }

        public int? TenantId { get; set ; }

        public Guid ItemId { get; private set; }
        public Item Item { get; private set; }
        public Guid? VendorId { get; private set; }
        public Vendor Vendor { get; private set; }

        public decimal Price { get; private set; }

        public Guid PurchasePriceId { get; private set; }
        public PurchasePrice PurchasePrice { get; private set; }


        public static PurchasePriceItem Create(int? tenantId, long creatorUserId, long currencyId, Guid itemId, Guid? vendorId, decimal price)
        {
            return new PurchasePriceItem()
            {
                Id = new Guid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                CurrencyId = currencyId,
                ItemId = itemId,
                VendorId = vendorId,
                Price = price,
    
            };
        }

        public static PurchasePriceItem Create(int? tenantId,long creatorUserId,long currencyId,Guid itemId, Guid? vendorId, decimal price,PurchasePrice purchasePrice)
        {
            var result = Create(tenantId,creatorUserId,currencyId,itemId, vendorId, price);
            result.PurchasePrice = purchasePrice;
            return result;
        }

        public static PurchasePriceItem Create(
          int? tenantId, long creatorUserId, long currencyId, Guid itemId, decimal price, Guid purchasePriceId, Guid? vendorId)
        {
            var result = Create(tenantId, creatorUserId, currencyId, itemId, vendorId, price);
            result.PurchasePriceId = purchasePriceId;
            return result;
        }
      

        public void Update(long lastModifiedUserId, long currencyId, Guid itemId, Guid? vendorId, decimal price,Guid itemPirceId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            CurrencyId = currencyId;
            ItemId = itemId;
            VendorId = vendorId;
            Price = price;
            PurchasePriceId = PurchasePriceId;
        }

    }
}
