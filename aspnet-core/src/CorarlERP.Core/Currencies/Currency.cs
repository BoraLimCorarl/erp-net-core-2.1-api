using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Currencies
{
    [Table("CarlErpCurrencies")]
    public class Currency : AuditedEntity<long>
    {
        public const int MaxCurrencyCodeLength = 16;
        public const int MaxCurrencyNameLength = 512;

        [Required]
        [MaxLength(MaxCurrencyCodeLength)]
        public string Code {get; private set;}

        [Required]
        [MaxLength(MaxCurrencyNameLength)]
        public string Name { get; set; }

        [Required]
        [MaxLength(MaxCurrencyCodeLength)]
        public string Symbol { get; private set; }

        [Required]
        [MaxLength(MaxCurrencyNameLength)]
        public string PluralName { get; set; }

        public static Currency Create(long? creatorUserId, string code,string name, string symbol, string pluralName)
        {
            return new Currency
            {
                CreatorUserId = creatorUserId,
                Code =code,
                Symbol = symbol,
                Name = name,
                PluralName = pluralName,

            };
        }

        public void Update(long? lastModifiedUserId, string code, string symbol, string name, string pluralName)

        {
            LastModifierUserId = lastModifiedUserId;
            Symbol = symbol;
            Code = code;
            Name = name;
            PluralName = pluralName;

        }
    }
}
