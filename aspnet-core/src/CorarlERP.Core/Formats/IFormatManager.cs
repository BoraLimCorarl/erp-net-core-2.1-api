using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Formats
{
   public  interface IFormatManager: IDomainService
    {
        Task<Format> GetAsync(long id, bool tracking = false);
        Task<IdentityResult> CreateAsync(Format @entity, bool checkDuplicate = true);
        Task<IdentityResult> UpdateAsync(Format entity, bool checkDuplicate = true);
    }
}
