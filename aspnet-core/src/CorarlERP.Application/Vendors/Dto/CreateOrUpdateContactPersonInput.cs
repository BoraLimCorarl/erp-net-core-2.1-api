using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Vendors.Dto
{
    public class CreateOrUpdateContactPersonInput
    {
        public Guid? Id { get; set; }
        //public int? TenantId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayNameAs { get; set; }
        public string PhoneNumber { get; set; }
        //public Guid VenderId { get; set; } //no need to provide vendor Id
        public bool IsPrimary { get; set; }
        public string Email { get; set; }
    }
    public class CreateOrUpdateContactPersonExcelInput
    {
        public Guid? Id { get; set; }
        //public int? TenantId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayNameAs { get; set; }
        public string PhoneNumber { get; set; }
        public VendorSummaryOutput Vendors { get; set; } 
        public bool IsPrimary { get; set; }
        public string Email { get; set; }
    }
}
