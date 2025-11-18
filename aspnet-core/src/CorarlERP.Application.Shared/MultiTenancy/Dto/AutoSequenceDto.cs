using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.MultiTenancy.Dto
{
   public class AutoSequenceDto
    {
        public Guid Id { get; set; }            
        public string DefaultPrefix { get; set; }
        public string SymbolFormat { get; set; }
        public string NumberFormat { get; set; }
        public bool CustomFormat { get; set; }
        public bool RequireReference { get; set; }
        public string YearFormatString { get; set; }
        public string LastAutoSequeneNumber { get; set; }
        public string DocumentTypeName { get; set; }
        public int DocumentTypeNumber { get; set; }
    }

    
}
