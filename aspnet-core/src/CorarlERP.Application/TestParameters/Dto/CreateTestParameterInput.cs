using CorarlERP.QCTests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.TestParameters.Dto
{
    public class CreateTestParameterInput
    {
        public string Name { get; set; }
        public TestSources TestSource { get; set; }
        public string LimitReferenceNote { get;  set; }
    }

}
