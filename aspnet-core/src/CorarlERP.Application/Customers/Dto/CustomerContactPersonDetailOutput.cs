using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Customers.Dto
{
    [AutoMapFrom(typeof(CustomerContactPerson))]
    public class CustomerContactPersonDetailOutput
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayNameAs { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsPrimary { get; set; }
        public string Email { get; set; }
    }
}
