using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ReportTemplates.Dto
{
    public class ReportTemplateAppInput
    {
        public string KeyName { get; set; }
    }


    public class GetReportTemplateInput : PagedSortedAndFilteredInputDto
    {
        public List<ReportCategory> ReportCategory { get; set; }
        public TemplateType Status { get; set; }
    }


    public class GetReportFindTemplateInput : GetReportTemplateInput
    {
        public List<ReportType> ReportType { get; set; }
        public List<long?> MemberId { get; set; }
        public List<Guid?> UserGroupId { get; set; }
    }
}
