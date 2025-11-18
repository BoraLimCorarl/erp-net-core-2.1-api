using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Currencies;
using CorarlERP.Items;

namespace CorarlERP.ItemPrices
{

    [Table("CarlErpItemPriceItems")]
    public class ItemPriceItem : AuditedEntity<Guid>, IMayHaveTenant
    {
       
        public long CurrencyId { get; private set; }
        public Currency Currency { get; private set; }

        public int? TenantId { get; set ; }

        public Guid ItemId { get; private set; }
        public Item Item { get; private set; }

        public decimal Price { get; private set; }

        public Guid ItemPriceId { get; private set; }
        public ItemPrice ItemPrice { get; private set; }


        public static ItemPriceItem Create(int? tenantId, long creatorUserId, long currencyId, Guid itemId, decimal price)
        {
            return new ItemPriceItem()
            {
                Id = new Guid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                CurrencyId = currencyId,
                ItemId = itemId,
                Price = price,
    
            };
        }

        public static ItemPriceItem Create(int? tenantId,long creatorUserId,long currencyId,Guid itemId,decimal price,ItemPrice itemPrice)
        {
            var result = Create(tenantId,creatorUserId,currencyId,itemId,price);
            result.ItemPrice = itemPrice;
            return result;
        }

        public static ItemPriceItem Create(
          int? tenantId, long creatorUserId, long currencyId, Guid itemId, decimal price, Guid itemPriceId)
        {
            var result = Create(tenantId, creatorUserId, currencyId, itemId, price);
            result.ItemPriceId = itemPriceId;
            return result;
        }
      

        public void Update(long lastModifiedUserId, long currencyId, Guid itemId,decimal price,Guid itemPirceId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            CurrencyId = currencyId;
            ItemId = itemId;
            Price = price;
            ItemPriceId = ItemPriceId;
        }

    }
}
