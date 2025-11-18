using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.Items.Dto
{
    public class CreateOrItemPropertyInput
    {
        public Guid? Id { get; set; }
        public long PropertyId { get; set; }
        public long? PropertyValueId { get; set; }

    }

    public class GenerateItemCodeInput
    {
        public string ItemCode { get; set; }
        public List<CreateOrItemPropertyInput> ItemProperties { get; set; }
        public int TenantId { get; set; }
        public long ItemTypeId { get; set; }

    }

}
