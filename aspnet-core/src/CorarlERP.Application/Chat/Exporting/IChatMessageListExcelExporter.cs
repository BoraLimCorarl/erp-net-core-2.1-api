using System.Collections.Generic;
using CorarlERP.Chat.Dto;
using CorarlERP.Dto;

namespace CorarlERP.Chat.Exporting
{
    public interface IChatMessageListExcelExporter
    {
        FileDto ExportToFile(List<ChatMessageExportDto> messages);
    }
}
