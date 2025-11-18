using Abp.AutoMapper;
using CorarlERP.ChartOfAccounts.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.BatchNos.Dto
{
    [AutoMapFrom(typeof(BatchNoFormula))]
    public class BatchNoFormulaDetailOutput
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public BatchNoFormulaPrePos StandardPrePos { get; set; }
     
        public string PrePosCode { get; set; }
        public string DateFormat { get; set; }
       
        public string FieldName { get; set; }
        public bool IsActive { get; set; }
        public string UserName { get; set; }
        public long? UserId { get; set; }
    }

    public class BatchNoFormulaOutput
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
