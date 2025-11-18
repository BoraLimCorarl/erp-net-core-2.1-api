using Amazon.S3.Model.Internal.MarshallTransformations;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.PhysicalCounts.Dto
{
    public class PhysicalCountSettingDto
    {
        public Guid? Id { get; set; }
        public bool No { get; set; }
        public bool ItemCode { get; set; }
        public bool LotName { get; set; }
        public bool DiffQty { get; set; }
    }
}
