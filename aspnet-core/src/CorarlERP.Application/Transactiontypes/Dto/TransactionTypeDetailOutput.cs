using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.TransactionTypes.Dto
{
    [AutoMapFrom(typeof(TransactionType))]
    public class TransactionTypeDetailOutput
    {
       public long Id { get; set; }
       public string TransactionTypeName { get; set; }        
       public bool IsActive { get; set; }
       public bool IsPOS { get; set; }
      
    }
    [AutoMapFrom(typeof(TransactionType))]
    public class TransactionTypeSummaryOutput
    {
        public long Id { get; set; }
        public string TransactionTypeName { get; set; }
       
    }
}
