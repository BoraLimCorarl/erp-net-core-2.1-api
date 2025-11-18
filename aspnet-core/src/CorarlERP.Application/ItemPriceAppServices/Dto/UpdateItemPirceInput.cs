using System;
namespace CorarlERP.ItemPriceAppServices.Dto
{
    public class UpdateItemPirceInput :CreateItemPriceInput
    {
       public Guid Id { get; set; }
    }
}
