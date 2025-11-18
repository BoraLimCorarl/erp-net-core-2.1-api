using System;

namespace CorarlERP.Withdraws.Dto
{
    public class UpdateWithdrawInput : CreateWithdrawInput
    {
        public Guid Id { get; set; }
        public DateTime? DateCompare { get; set; }
    }
}
