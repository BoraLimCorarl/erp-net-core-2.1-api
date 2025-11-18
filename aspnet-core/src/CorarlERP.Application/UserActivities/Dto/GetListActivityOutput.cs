using System;
namespace CorarlERP.UserActivities.Dto
{
    public class GetListActivityOutput
    {
        public long Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public bool IsReport { get; set; }
    }
}
