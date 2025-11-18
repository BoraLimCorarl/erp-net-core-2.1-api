using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.ItemTypes
{
    public interface IItemTypeMannager: IDomainService
    {
        Task<ItemType> GetAsync(long id, bool tracking = false);
        Task<IdentityResult>CreateAsync(ItemType @entity, bool checkDuplicate = true);
        Task<IdentityResult>UpdateAsync(ItemType @entity, bool checkDuplicate = true);
    }
}
