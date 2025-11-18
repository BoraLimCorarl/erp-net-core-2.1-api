using System.Collections.Generic;

namespace CorarlERP.Sessions.Dto
{
    public class GetCurrentLoginInformationsOutput
    {
        public UserLoginInfoDto User { get; set; }

        public TenantLoginInfoDto Tenant { get; set; }

        public ApplicationInfoDto Application { get; set; }
        public List<LotSessionOutput> Lots { get; set; }

    }

    public class LotSessionOutput { 
    
       public long Id { get; set; }
       public bool IsActive { get; set; }
       public long LocationId { get; set; }
       public string LotName { get; set; }    
       public string LocationName { get; set; }
    }


    public class GetCurrentLoginInformationsSummaryOutput
    {
        public UserLoginSummaryInfoDto User { get; set; }

        public TenantLoginSummaryInfoDto Tenant { get; set; }


    }

}