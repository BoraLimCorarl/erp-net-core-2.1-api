using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Classes.Dto
{
    [AutoMapFrom(typeof(Class))]
    public class ClassDetailOutput
    {
       public long Id { get; set; }
       public string ClassName { get; set; } 
       public bool ClassParent { get; set; }
       public long? ParentClassId { get; set; }
       public bool IsActive { get; set; }
       public ClassSummaryOutput ParentClass { get; set; }
    }
    [AutoMapFrom(typeof(Class))]
    public class ClassSummaryOutput
    {
        public long Id { get; set; }
        public string ClassName { get; set; }
       
    }
}
