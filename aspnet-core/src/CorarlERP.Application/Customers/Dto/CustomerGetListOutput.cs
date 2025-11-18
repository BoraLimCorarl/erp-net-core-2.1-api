using Abp.AutoMapper;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.LayaltyAndMemberships;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Customers.Dto
{
    [AutoMapFrom(typeof(Customer))]
    public class CustomerGetListOutput
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }       
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Guid? AccountId { get; set; }      
        public ChartAccountSummaryOutput Account { get; set; }
    }

    [AutoMapFrom(typeof(Customer))]
    public class GetListFindOutput
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Guid? AccountId { get; set; }
        public ChartAccountSummaryOutput Account { get; set; }
        public bool IsWalkIn { get; set; }
        public long? CustomerTypeId { get; set; }

        public CAddress BillingAddress { get; set; }
        public CAddress ShippingAddress { get; set; }
        public bool SameAsShippngAddress { get; set; }
    }


    public class GetCustomerByCardId
    {
        public Guid? CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }      
        public Guid? AccountId { get; set; }    
        public bool IsWalkIn { get; set; }
        public long? CustomerTypeId { get; set; }
        public CardStatus CardStatus { get; set; }

    }
    public class GetCustomerByCardIdInput
    {
       public string CardId { get; set; }

    }
}
