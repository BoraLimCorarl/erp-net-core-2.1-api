using CorarlERP.Dto;
using System;

namespace CorarlERP.Common.Dto
{
    public class FindUsersInput : PagedAndFilteredInputDto
    {
        public int? TenantId { get; set; }
    }
    public class FindEditionInput { 
    
        public Guid? PackageId { get; set; }
    }

}