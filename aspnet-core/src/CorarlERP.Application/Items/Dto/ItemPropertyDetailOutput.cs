using Abp.AutoMapper;
using CorarlERP.PropertyValues.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Items.Dto
{
    [AutoMapFrom(typeof(ItemProperty))]
    public class ItemPropertyDetailOutput
    {
        public Guid Id { get; set; }
        public long PropertyId { get; set; }
        public PropertyDetailOutput Property {get;set;}
        public long? PropertyValueId { get; set; }
        public PropertyValueDetailOutput PropertyValue { get; set; }
    }
}
