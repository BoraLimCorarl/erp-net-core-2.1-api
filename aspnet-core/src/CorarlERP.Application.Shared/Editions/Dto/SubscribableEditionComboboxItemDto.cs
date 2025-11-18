using Abp.Application.Services.Dto;

namespace CorarlERP.Editions.Dto
{
    public class SubscribableEditionComboboxItemDto : ComboboxItemDto
    {
        public bool? IsFree { get; set; }
        public int? TrialDayCount { get; set; }
        public decimal? DailyPrice { get; set; }
        public bool IsPaid { get; set; }

        public SubscribableEditionComboboxItemDto(string value, string displayText, bool? isFree,
            int? trialDayCount, decimal? dailyPrice, bool isPaid) : base(value, displayText)
        {
            IsFree = isFree;
            TrialDayCount = trialDayCount;
            DailyPrice = dailyPrice;
            IsPaid = isPaid;
        }
    }
}