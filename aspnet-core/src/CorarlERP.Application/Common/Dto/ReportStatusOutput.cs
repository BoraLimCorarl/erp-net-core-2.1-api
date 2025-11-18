using CorarlERP.ReportTemplates;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Common.Dto
{
    public class ReportStatusOutput
    {

    }

    public class GetReportStatusOutput
    {
        public ReportCategory Status { get; set; }

        public string code { get; set; }

    }
}
