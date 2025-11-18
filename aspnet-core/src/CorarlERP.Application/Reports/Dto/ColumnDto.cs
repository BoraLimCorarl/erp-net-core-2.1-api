using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Reports.Dto
{
   public class ColumnDto
    {
        public int SortOrder { get; set; }
        public string ColumnName { get; set; }
        public string ColumnTitle { get; set; }
        public decimal ColumnLength { get; set; }
        public bool Visible { get; set; }
    }
}
