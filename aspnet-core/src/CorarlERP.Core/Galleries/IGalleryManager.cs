using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Galleries
{
    public interface IGalleryManager : IDomainService
    {
        Task CreateAsync(Gallery gallery);
        Task<IdentityResult> DeleteAsync(Gallery gallery);
        Task<Gallery> GetAsync(Guid id, bool tracking = false);
    }
}
