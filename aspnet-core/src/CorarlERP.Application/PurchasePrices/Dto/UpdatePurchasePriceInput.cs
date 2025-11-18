using System;
namespace CorarlERP.PurchasePrices.Dto
{
    public class UpdatePurchasePriceInput :CreatePurchasePriceInput
    {
       public Guid Id { get; set; }
    }
}
