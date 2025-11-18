using System;
using Abp.Runtime.Validation;
using CorarlERP.Currencies.Dto;
using CorarlERP.Dto;

namespace CorarlERP.Exchanges.Dto
{
    public class GetListExchangeInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "FromDate";
            }
        }
    }

    public class GetExchangeInput {

        public DateTime Date { get; set; }

        public long? FromCurrency { get; set; }

        public long? ToCurrency { get; set; }
    }

    public class FindExchangeInput 
    {
        public DateTime Date { get; set; }
    }

    public class GetExchangeRateDto
    {
        public Guid Id { get; set; }

        public long FromCurrencyId { get; set; }
        public string FromCurrencyCode { get; set; }

        public long ToCurrencyId { get; set; }
        public string ToCurrencyCode { get; set; }

        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
        public bool IsInves { get; set; }

    }

}
