using Abp.AutoMapper;
using CorarlERP.AutoSequences;
using CorarlERP.enumStatus;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.MultiTenancy.Dto
{
    public class GetLsitAutoSequenceDetailOutput
    {
        public Guid Id { get; set; }
        public DocumentType DocumentType { get; set; }
        public string DocumentTypeName { get; set; }
        public string DefaultPrefix { get;  set; }
        public string SymbolFormat { get;  set; }
        public string NumberFormat { get;  set; }
        public bool CustomFormat { get;  set; }
        public bool RequireReference { get; set; }
        public YearFormat YearFormat { get;  set; }
        public DocumentTypeGroup KeyName { get;  set; }
        public string YearFormatString { get; set; }
        public string LastAutoSequeneNumber { get; set; }
    }

    public class AutoSequenceGroupOutput
    {
        public DocumentTypeGroup KeyName { get; set; }
        public string GroupByName { get; set; }
        public List<GetLsitAutoSequenceDetailOutput> Items { get; set; }
    }
}
