using Abp.AutoMapper;
using CorarlERP.Organizations.Dto;

namespace CorarlERP.Models.Users
{
    [AutoMapFrom(typeof(OrganizationUnitDto))]
    public class OrganizationUnitModel : OrganizationUnitDto
    {
        public bool IsAssigned { get; set; }
    }
}