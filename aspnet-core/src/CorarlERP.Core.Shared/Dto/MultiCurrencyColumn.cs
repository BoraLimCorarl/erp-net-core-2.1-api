using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Dto
{
    public class MultiCurrencyColumn
    {
        public long CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Balance { get; set; }
        public decimal LastPayment { get; set; }
    }

    public class MultiCurrencyPagedResultDto<T> : PagedResultDto<T>
    {
        public List<string> Currencies { get; set; }
    }
}
