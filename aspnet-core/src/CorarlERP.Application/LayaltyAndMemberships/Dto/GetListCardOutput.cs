using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.LayaltyAndMemberships.Dto
{
    [AutoMapFrom(typeof(Card))]
    public  class GetListCardOutput
    {      
        public Guid? Id { get; set; }
      
        public string CardId { get;  set; }
        public string CustomerName { get;  set; }
        public Guid? CustomerId { get;  set; }
        public string Remark { get;  set; }
        public string CardStatusName { get;  set; }
        public CardStatus CardStatus { get; set; }
        public string CardNumber { get; set; }
        public string SerialNumber { get; set; }
        public string CustomerCode { get; set; }
    }
}
