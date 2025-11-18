using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.CashFlowTemplates.Dto
{
    public class CashFlowAccountGroupDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public string Description { get; set; }
    }
}
