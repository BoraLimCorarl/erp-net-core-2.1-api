using Amazon.EventBridge;
using CorarlERP.Locks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.EventBridge.Dto
{
    public class CreateEventBridgeInput
    {
        public string Expression { get; set; }    
        public string Description { get; set; }
        public long? JobId { get; set; }
        public SyncDataInput Input { get; set; }

    }
    public class DeleteEventBridgeInput
    {
        public long? JobId { get; set; }
    }

    public class SyncDataInput
    {
        //public DateTime ScheduleTime { get; set; }
        //public ScheduleDate ScheduleDate { get; set; }
        public int TenantId { get; set; }   
        public long? UserId { get; set; }    
       // public bool ThowExeption { get; set; }
        public long? JobId { get; set; }
    }
}
