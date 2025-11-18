using System;
namespace CorarlERP.UserActivities.Dto
{
    public class GetDetailListUserActivityOutput
    {
        public string Description { get; set; }

        public string Browser { get; set; }

        public string Duration { get; set; }

        public DateTime  Time { get; set; }

        public string User { get; set; }
        public long UserId { get; set; }

        public string Activity { get; set; }

        public bool ErrorState { get; set; }
        public string Transsaction { get; set; }
    }
}
