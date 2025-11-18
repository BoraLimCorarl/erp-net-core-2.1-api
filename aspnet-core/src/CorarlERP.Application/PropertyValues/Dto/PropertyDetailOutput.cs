using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.PropertyValues.Dto
{
    [AutoMapFrom(typeof(Property))]
    public class PropertyDetailOutput
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsUnit { get; set; }
        public bool IsRequired { get; set; }
        public bool IsStatic { get; set; }
        public bool IsItemGroup { get; set; }
        public bool IsStandardCostGroup { get; set; }
      //  public List<PropertyValueDetailOutput> Value { get; set; }
    }

    public class PropertyOutput
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsUnit { get; set; }
        public bool IsRequired { get; set; }
        public  bool IsStatic { get; set; }
        public bool IsItemGroup { get; set; }
        public List<FindPropertyValueDetailOutput> Value { get; set; }
    }
}
