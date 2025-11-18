using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Customers.Dto
{
    public class GetCustomerListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool? IsActive { get; set; }
        public bool UsePagination { get; set; }
        public List<long?> CustomerTypes { get; set; }
        public List<long?> Locations { get; set; }
        public bool IsWalkIn { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CustomerName";
            }

        }
    }
    public class CheckExistCustomerInput
    {
        public long? LocationId { get; set; }
        public Guid? CustomerId { get; set; }
    }

}
