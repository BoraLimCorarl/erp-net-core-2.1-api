using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.Vendors
{
    public class ContactPersonManager : CorarlERPDomainServiceBase, IContactPersonManager
    {
        private readonly IRepository<ContactPreson, Guid> _contactPersonRepository;
        public ContactPersonManager(IRepository<ContactPreson, Guid> contactPersonRepository)
        {
            _contactPersonRepository = contactPersonRepository;
        }
        private async Task CheckDuplicateContactPerson(ContactPreson @entity)
        {
            var @old = await _contactPersonRepository.GetAll().AsNoTracking()
                           .Where(u => u.Id == entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateContactPerson")); //this contact person is created already
            }
        }
        public async virtual Task<IdentityResult> CreateAsync(ContactPreson cEntity)
        {
            await CheckDuplicateContactPerson(cEntity);
            await _contactPersonRepository.InsertAsync(cEntity);
            return IdentityResult.Success;
        }

        public async virtual Task<ContactPreson> GetAsync(Guid id, bool tracking = true)
        {
            var @query = tracking ? _contactPersonRepository.GetAll() : _contactPersonRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(ContactPreson entity)
        {
            await _contactPersonRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(ContactPreson entity)
        {
            await _contactPersonRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
