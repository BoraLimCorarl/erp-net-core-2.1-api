using CorarlERP.Currencies;
using CorarlERP.Formats;
using CorarlERP.ItemTypes;
using System.Collections.Generic;

namespace CorarlERP
{
    public interface IDefaultValues
    {
        List<Currency> Currencies { get; set; }
        List <ItemType> ItemTypes { get; set; }
        List <Format> Formats { get; set; }
    }

}
