using System;
using CorarlERP.Currencies.Dto;

namespace CorarlERP.Exchanges.Dto
{
    public class CreateOrUpdateExchangeInput
    {
        public Guid? Id { get; set; }

        public long FromCurrencyId { get; set; }
        public CurrencyDetailOutput FromCurrency { get; set; }

        public long ToCurrencyId { get; set; }
        public CurrencyDetailOutput ToCurrency { get; set; }

        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
        public bool IsInves { get; set; }

    }
}
