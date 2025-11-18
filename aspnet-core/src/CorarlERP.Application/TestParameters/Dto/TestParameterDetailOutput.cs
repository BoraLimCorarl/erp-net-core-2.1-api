using Abp.AutoMapper;
using CorarlERP.QCTests;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.TestParameters.Dto
{
    [AutoMapFrom(typeof(TestParameter))]
    public class TestParameterDetailOutput
    {
       public long Id { get; set; }
        public string Name { get; set; }
        public TestSources TestSource { get; set; }
        public string LimitReferenceNote { get; set; }
        public bool IsActive { get; set; }
      
    }

    [AutoMapFrom(typeof(TestParameter))]
    public class TestParameterSummaryOutput
    {
        public long Id { get; set; }
        public string Name { get; set; }
       
    }
    
}
