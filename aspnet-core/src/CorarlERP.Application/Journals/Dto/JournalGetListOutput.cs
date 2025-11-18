using Abp.AutoMapper;
using CorarlERP.Dto;
using System;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Journals.Dto
{
    [AutoMapFrom(typeof(Journal))]
    public class JournalGetListOutput
    {
        public string JournalNo { get; set; }
        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public string Memo { get; set; }

        public long? LocationId { get; set; }
        public string LocationName { get; set; }

       // public bool? IsActive { get; set; }

        public Guid Id { get; set; }

        public DateTime Date { get; set; }
        public UserDto User { get; set; }

        public TransactionStatus StatusCode { get; set; }
        public String StatusName { get; set; }
        public long? CreationTimeIndex { get; set; }
      
    }

}
