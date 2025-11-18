using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Formats.Dto
{
    [AutoMapFrom(typeof(Format))]
   public class FormatDetailOutput
    {
        public long Id { get; set; }
        public string Key { get; set; }
        public string Web { get; set; }
        public string Name { get; set; }
    }
}
