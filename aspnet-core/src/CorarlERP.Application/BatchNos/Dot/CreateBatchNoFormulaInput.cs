using CorarlERP.BatchNos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.BatchNos.Dto
{
    public class CreateOrUpdateBatchNoFormulaInput
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public BatchNoFormulaPrePos StandardPrePos { get; set; }

        public string PrePosCode { get; set; }
        public string DateFormat { get; set; }

        public string FieldName { get; set; }
        public bool IsActive { get; set; }
    }
}
