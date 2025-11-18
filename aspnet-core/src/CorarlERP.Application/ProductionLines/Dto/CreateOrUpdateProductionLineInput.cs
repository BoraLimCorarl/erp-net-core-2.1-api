using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.ProductionLines.Dto
{
    public class CreateOrUpdateProductionLineInput
    {
        public long Id { get; set; }
        public string ProductionLineName { get; set; }
        public string Memo { get; set; }
        public bool IsActive { get; set; }
    }
}
