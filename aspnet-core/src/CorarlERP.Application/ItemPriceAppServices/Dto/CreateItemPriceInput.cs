using System;
using System.Collections.Generic;

namespace CorarlERP.ItemPriceAppServices.Dto
{
    public class CreateItemPriceInput
    {
       public long? CustomerTypeId { get; set; }
       public long? SaleTypeId { get; set; }
       public long? LocationId { get; set; }
       public List<CreateItemPriceItemInput> ItemPriceItems { get; set; }

    }
}
