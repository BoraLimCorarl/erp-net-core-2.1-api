using Abp.AutoMapper;
using CorarlERP.QCTests;
using System.Collections.Generic;

namespace CorarlERP.QCTestTemplates.Dto
{
    [AutoMapFrom(typeof(QCTestTemplate))]
    public class QCTestTemplateDetailOutput
    {
       public long Id { get; set; }
        public string Name { get; set; }
        public TestSources TestSource { get; set; }
        public bool DetailEntryRequired { get; set; }
        public List<QCTestTemplateParameterInput> QCTestTemplateParameters { get; set; }
        public bool IsActive { get; set; }
      
    }

    [AutoMapFrom(typeof(QCTestTemplate))]
    public class QCTestTemplateSummaryOutput
    {
        public long Id { get; set; }
        public string Name { get; set; }
       
    }
    
}
