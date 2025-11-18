using System.Collections.Generic;
using CorarlERP.Authorization.Users.Dto;
using CorarlERP.Dto;

namespace CorarlERP.Authorization.Users.Exporting
{
    public interface IUserListExcelExporter
    {
        FileDto ExportToFile(List<UserListDto> userListDtos);
    }
}