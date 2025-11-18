using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP
{
    public interface IDefaultData
    {
        IDictionary<string, List<string>> StaticPermissions { get; set; }
        IDictionary<string, string> UrlMappings { get; set; }
    }
}
