using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP
{
    public class DefaultData : IDefaultData, ISingletonDependency
    {

        public DefaultData()
        {

        }

        public IDictionary<string, List<string>> StaticPermissions { get; set; }
        public IDictionary<string, string> UrlMappings { get; set; }
    }
}