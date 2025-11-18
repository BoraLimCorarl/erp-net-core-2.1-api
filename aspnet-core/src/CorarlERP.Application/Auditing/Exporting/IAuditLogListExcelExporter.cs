using System.Collections.Generic;
using CorarlERP.Auditing.Dto;
using CorarlERP.Dto;

namespace CorarlERP.Auditing.Exporting
{
    public interface IAuditLogListExcelExporter
    {
        FileDto ExportToFile(List<AuditLogListDto> auditLogListDtos);

        FileDto ExportToFile(List<EntityChangeListDto> entityChangeListDtos);
    }
}
