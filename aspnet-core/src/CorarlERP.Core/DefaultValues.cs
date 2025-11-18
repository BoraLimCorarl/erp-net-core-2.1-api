using Abp.Dependency;
using CorarlERP.Currencies;
using System.Collections.Generic;
using CorarlERP.ItemTypes;
using CorarlERP.Formats;

namespace CorarlERP
{
    public class DefaultValues: IDefaultValues, ISingletonDependency
    {
        public DefaultValues()
        {
        }

        public List<Currency> Currencies { get; set; }
        public List<ItemType> ItemTypes { get; set; }
        public List<Format> Formats { get; set; }
    }
}
