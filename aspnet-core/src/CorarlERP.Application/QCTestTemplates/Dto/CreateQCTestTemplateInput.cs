using CorarlERP.QCTests;
using System;
using System.Collections.Generic;

namespace CorarlERP.QCTestTemplates.Dto
{
    public class CreateQCTestTemplateInput
    {
        public string Name { get; set; }
        public TestSources TestSource { get; set; }
        public bool DetailEntryRequired { get; set; }
        public List<QCTestTemplateParameterInput> QCTestTemplateParameters { get; set; }
    }

    public class QCTestTemplateParameterInput
    {
        public long? Id { get; set; }
        public string LimitReferenceNoteOverride { get; set; }

    }
}
