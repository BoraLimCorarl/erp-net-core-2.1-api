using Abp.AutoMapper;
using CorarlERP.AccountTypes.Dto;
using CorarlERP.Taxes.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ChartOfAccounts.Dto
{
    [AutoMapFrom(typeof(ChartOfAccount))]
    public class ChartAccountDetailOutput
    {
        public Guid Id { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }

        public TaxDetailOutput Tax { get; set; }
        public long TaxId { get; set; }

        public AccountTypeDetailOutput AccountType { get; set; }
        public long AccountTypeId { get; set; }
        public ChartAccountSummaryOutput ParentAccount { get; set; }

        public string Description { get; set; }
        public bool IsActive { get; set; }
        public decimal? Balance { get; internal set; }

        public SubAccountType? SubAccountType { get; set; }
    }

    [AutoMapFrom(typeof(ChartOfAccount))]
    public class ChartAccountSummaryOutput
    {
        public Guid Id { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
    }

    
    public class GetAccountOutput
    {
        public Guid Id { get; set; }
        public string AccountName { get; set; }
    }

    public class GetAccountWithTypeOutput
    {
        public Guid Id { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public long AccountTypeId { get; set; }
        public string AccountTypeName { get; set; }
        public TypeOfAccount TypeCode { get; set; }
    }

}
