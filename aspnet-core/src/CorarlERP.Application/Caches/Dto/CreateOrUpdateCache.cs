using Abp.AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Caches.Dto
{
    [AutoMapFrom(typeof(Cache))]
    public class CreateOrUpdateCache
    {
        public long Id { get; set; }
        public string KeyName { get; set; }
        public string KeyValue { get; set; }

        //public string[] KeyValue {
        //    get { return Value == null ? null : JsonConvert.DeserializeObject<string[]>(Value); }
        //    set { Value = JsonConvert.SerializeObject(value); }
        //}

    }
}
