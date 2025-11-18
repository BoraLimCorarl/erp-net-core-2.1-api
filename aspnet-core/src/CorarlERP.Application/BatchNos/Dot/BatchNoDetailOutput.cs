using Abp.AutoMapper;
using System;

namespace CorarlERP.BatchNos.Dto
{
    [AutoMapFrom(typeof(BatchNo))]
    public class BatchNoDetailOutput
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string Code { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }

}
