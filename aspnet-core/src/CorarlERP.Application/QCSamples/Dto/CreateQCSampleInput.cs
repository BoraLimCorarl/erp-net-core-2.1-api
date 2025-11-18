using System;

namespace CorarlERP.QCSamples.Dto
{
    public class CreateQCSampleInput
    {
        public string SampleId { get; set; }
        public string SourceDoc { get; set; }
        public DateTime SampleDate { get; set; }
        public Guid ItemId { get; set; }
        public long LocationId { get; set; }
        public string Remark { get; set; }
    }

}
