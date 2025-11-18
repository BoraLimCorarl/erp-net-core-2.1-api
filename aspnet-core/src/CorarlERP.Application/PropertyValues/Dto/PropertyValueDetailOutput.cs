using Abp.AutoMapper;
using CorarlERP.PropertyValues;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.PropertyValues.Dto
{
    [AutoMapFrom(typeof(PropertyValue))]
    public class PropertyValueDetailOutput
    {
        public long? Id { get; set; }
        public string Value { get; set; }
        public PropertyDetailOutput Property { get; set; }
        public bool IsActive { get; set; }
        public decimal NetWeight { get; set; }
        public bool IsDefault { get; set; }
        public string Code { get; set; }
        public bool IsBaseUnit { get; set; }
        public long? BaseUnitId { get; set; }
        public string BaseUnitName { get; set; }
        public decimal Factor { get; set; }
    }
    [AutoMapFrom(typeof(PropertyValue))]
    public class FindPropertyValueDetailOutput
    {
        public long? Id { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
        public long? PropertyId { get; set; }
        public decimal NetWeight { get; set; }
        public bool IsDefault { get; set; }
        public long? PropertyValueId { get; set; }
        public string Code { get; set; }
    }

    public class PropertyValueSummarryOutput { 
        public long? Id { get; set; }
        public string Code { get; set; }
        public long? PropertyId { get; set; }
        public string ProperyName { get; set; }
    
    }


}
