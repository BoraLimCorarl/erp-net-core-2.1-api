using Abp.AutoMapper;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.CustomerTypes;
using CorarlERP.CustomerTypes.Dto;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Customers.Dto
{
    [AutoMapFrom(typeof(Customer))]
    public class CustomerDetailOutput
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Website { get; set; }
        public CAddress BillingAddress { get; set; }
        public CAddress ShippingAddress { get; set; }
        public string Remark { get; set; }
        public int? TenantId { get; set; }
        public bool SameAsShippngAddress { get; set; }
        public long? CustomerTypeId { get; set; }
        public CustomerTypeSummaryOutput CustomerType { get; set; }
        public Guid? AccountId { get; set; }
        public ChartAccountSummaryOutput Account { get; set; }
        public bool IsWalkIn { get; set; }
        public Member Member { get; set; }
        public List<GroupItems> UserGroups { get; set; }
        public List<CustomerContactPersonDetailOutput> CustomerContactPersons { get; set; }
    }

    [AutoMapFrom(typeof(Customer))]
    public class CustomerSummaryOutput
    {
        public Guid Id { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public CAddress shippingAddress { get; set; }
        public Guid? AccountId { get; set; }
        public ChartAccountSummaryOutput Account { get; set; }

    }

    [AutoMapFrom(typeof(Customer))]
    public class CustomerSummaryDetailOutput
    {
        public Guid Id { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public Guid? AccountId { get; set; }
        public bool IsWalkIn { get; set; }
        public long? CustomerTypeId { get; set; }
        public ChartAccountSummaryOutput Account { get; set; }

    }

    
    public class GetCustomerWithCodeOutput
    {
        public Guid Id { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
    }

    public class GetCustomerOutput
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
    }

    public class GetCustomerWithTypeOutput
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTypeName { get; set; }
    }

    public class GetCustomerWithCodeTypeOutput
    {
        public Guid Id { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTypeName { get; set; }
    }

}
