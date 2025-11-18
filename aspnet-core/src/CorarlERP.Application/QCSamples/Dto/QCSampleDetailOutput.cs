using Abp.AutoMapper;
using CorarlERP.QCTests;
using System;

namespace CorarlERP.QCSamples.Dto
{
    [AutoMapFrom(typeof(QCSample))]
    public class QCSampleDetailOutput
    {
        public Guid Id { get; set; }
        public string SampleId { get; set; }
        public string SourceDoc { get; set; }
        public DateTime SampleDate { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public string Remark { get; set; }

    }

}
