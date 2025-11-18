using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.BatchNos.Dto
{
    public class GenerateBatchNoOutput
    {
        public Guid ItemId { get; set; }
        public string BatchNumber { get; set; }
    }
}
