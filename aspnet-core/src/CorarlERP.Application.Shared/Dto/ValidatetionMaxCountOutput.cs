using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Dto
{
    public class ValidationCountOutput
    {
        public int MaxCount { get; set; }
        public int MaxCurrentCount { get; set; }
    }
    public class GetCustomItemCodeSetting
    {
        public bool CustomCode { get; set; }
        public bool Formula { get; set; }
    }
}
