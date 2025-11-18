using System;
namespace CorarlERP.PaymentMethods.Dto
{
    public class CreatePaymentMethodBaseInput
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool IsDefault { get; set; }

    }
}
