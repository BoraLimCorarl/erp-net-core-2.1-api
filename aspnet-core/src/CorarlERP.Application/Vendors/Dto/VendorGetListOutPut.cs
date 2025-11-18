using Abp.AutoMapper;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Vendors.Dto
{
    [AutoMapFrom(typeof(Vendor))]
    public class VendorGetListOutPut
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public CAddress BillingAddress { get; set; }
        public CAddress ShippingAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Guid? AccountId { get; set; }
        public bool SameAsShippngAddress { get; set; }
        public ChartAccountSummaryOutput ChartOfAccount { get; set; }
    }
}
