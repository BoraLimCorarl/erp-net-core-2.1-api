using System.Collections.Generic;
using MvvmHelpers;
using CorarlERP.Models.NavigationMenu;

namespace CorarlERP.Services.Navigation
{
    public interface IMenuProvider
    {
        ObservableRangeCollection<NavigationMenuItem> GetAuthorizedMenuItems(Dictionary<string, string> grantedPermissions);
    }
}