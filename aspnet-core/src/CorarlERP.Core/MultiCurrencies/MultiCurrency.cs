using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CorarlERP.Currencies;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.MultiCurrencies
{
    [Table("CarlErpMultiCurrencies")]
    public class MultiCurrency : AuditedEntity<long>, IMayHaveTenant
    {
      
        public long? CurrencyId { get; private set; }
        public Currency  Currency { get; private set; }

        public int? TenantId { get; set; }

        public static MultiCurrency Create(int? tenantId, long? creatorUserId, long? currencyId)
        {
            return new MultiCurrency
            {
                CreatorUserId = creatorUserId,
                CurrencyId = currencyId,
                TenantId =  tenantId,

            };
        }

        public void Update(long? lastModifiedUserId,long? currencyId )

        {
            LastModifierUserId = lastModifiedUserId;
            CurrencyId = currencyId;
        }
    }
}
