using Abp.AutoMapper;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.VendorTypes.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Vendors.Dto
{
    [AutoMapFrom(typeof(Vendor))]
    public class VendorDetailOutput
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Websit { get; set; }
        public CAddress BillingAddress { get; set; }
        public CAddress ShippingAddress { get; set; }
        public string Remark { get; set; }
        public int? TenantId { get; set; }
        public bool SameAsShippngAddress { get; set; }

        public Guid? AccountId { get; set; }
        public ChartAccountSummaryOutput ChartOfAccount { get;set ;}
        public VendorTypeDetailOutput VendorType { get; set; }
        public long? VendorTypeId { get; private set; }
        public List<ContactPersonDetailOut> ContactPersons { get; set; }

        public Member Member { get; set; }
        public List<GroupItems> UserGroups { get; set; }
    }

    [AutoMapFrom(typeof(Vendor))]
    public class VendorSummaryOutput
    {
        public Guid Id { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public Guid AccountId { get; set; }
        public long? VendorTypeId { get; set; }
        

    }

    [AutoMapFrom(typeof(Vendor))]
    public class VendorSummaryDetailOutput
    {
        public Guid Id { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public Guid? AccountId { get; set; }
        public ChartAccountSummaryOutput ChartOfAccount { get; set; }

    }

    public class GetVendorOutput
    {
        public Guid Id { get; set; }
        public string VendorName { get; set; }
    }

    public class GetVendorWithCodeOutput
    {
        public Guid Id { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
    }

    public class GetVendorWithCodeTypeOutput
    {
        public Guid Id { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string VendorTypeName { get; set; }
    }
}
