using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.BatchNos.Dto
{
 
    public class GenerateBatchNoInput
    {
        public List<Guid> ItemIds { get; set; }
        public DateTime Date { get; set; }
        public bool Standard { get; set; }
    }

}
