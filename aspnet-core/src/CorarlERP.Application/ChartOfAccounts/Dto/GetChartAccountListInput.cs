using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.ChartOfAccounts.Dto
{
    public class GetChartAccountListInput: PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<long> AccountTypes { get; set; }
        public bool? IsActive { get; set; }
        public List<Guid> ListChartOfAccounts { get; set; }
        public bool UsePagination { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                this.Sorting = "accountCode, accountName";
            }
        }
    }
}
