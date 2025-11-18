using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.LayaltyAndMemberships
{
   public interface ICardManager : IDomainService
    {
        Task<Card> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(Card @entity);
        Task<IdentityResult> RemoveAsync(Card @entity);
        Task<IdentityResult> UpdateAsync(Card @entity);
        Task<IdentityResult> DisableAsync(Card @entity);
        Task<IdentityResult> EnableAsync(Card @entity);
        Task<IdentityResult> DeactivateAsync(Card @entity);
    }
}
