using Abp.AutoMapper;
using CorarlERP.ChartOfAccounts;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.AccountTypes.Dto
{
    [AutoMapFrom(typeof(AccountType))]
    public class AccountTypeDetailOutput
    {
        public long Id { get; set; }
        public string AccountTypeName { get; set; }
        public string Type { get; set; }
        public TypeOfAccount TypeCode { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
