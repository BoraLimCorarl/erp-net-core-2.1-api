using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.CustomerCredits.Dto
{
    public class ListCustomerCreditInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {

        public List<Guid> Customers { get; set; }
        public List<long?> Locations { get; set; }
        public List<Guid> ReceivePaymentId { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }

    public class GetCustomerCreditInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid> Customers { get; set; }
        public List<long?> Locations { get; set; }
        public List<long?> Lots { get; set; }
        public List<Guid?> Items { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }

    public class GetNewCustomerCreditInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid> Customers { get; set; }
        public List<long?> Locations { get; set; }
        public List<long?> Lots { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }
}
