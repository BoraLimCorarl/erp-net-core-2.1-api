using Abp.AutoMapper;
using CorarlERP.ChartOfAccounts.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Journals.Dto
{
    [AutoMapFrom(typeof(JournalItem))]
    public class JournalItemDetailOutput
    {
        public Guid Id { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public Guid AccountId { get; set; }
        public ChartAccountSummaryOutput Account { get; set; }

        public string Description { get; set; }

        public DateTime CreationTime { get; set; }

        public string ItemName { get; set; }
        public Guid? ItemId { get; set; }
        public Guid JournalId { get; set; }
        public int TenantId { get; set; }
    }

    [AutoMapFrom(typeof(JournalItem))]
    public class JournalItemSummaryOutput
    {
        public Guid Id { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public string Description { get; set; }


    }


}
