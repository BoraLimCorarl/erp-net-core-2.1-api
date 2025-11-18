using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Galleries
{
    public class GalleryManager: CorarlERPDomainServiceBase, IGalleryManager
    {
        private readonly IRepository<Gallery, Guid> _galleryRepository;
        public GalleryManager(IRepository<Gallery, Guid> galleryRepository)
        {
            _galleryRepository = galleryRepository;
        }
        public async Task<IdentityResult> UpdateAsync(Gallery gallery)
        {
            await _galleryRepository.UpdateAsync(gallery);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(Gallery gallery)
        {
            await _galleryRepository.DeleteAsync(gallery);
            return IdentityResult.Success;
        }

        public async Task CreateAsync(Gallery gallery)
        {
            var isExisting = await CheckDuplication(gallery);
            if (isExisting)
            {
                var ext = Path.GetExtension(gallery.Name);
                var name = Path.GetFileNameWithoutExtension(gallery.Name);
                var total = await _galleryRepository.GetAll()
                .Where(u => u.Name.Contains(name) && u.CreatorUserId == gallery.CreatorUserId)
                .AsNoTracking()
                .CountAsync();
                var newName = name + "_(" + total.ToString() + ")" + ext;
                gallery.updateName(newName);
            }
            await _galleryRepository.InsertAsync(gallery);
        }
      
        public async Task<Gallery> GetAsync(Guid id, bool tracking = false)
        {
            var query = tracking ? _galleryRepository.GetAll() : _galleryRepository.GetAll().AsNoTracking();
            var result = await query.Where(u => u.Id == id).FirstOrDefaultAsync(); 
            return result;
        }

        private async Task<bool> CheckDuplication(Gallery gallery)
        {
            var data = await _galleryRepository.GetAll().AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id != gallery.Id &&
            u.CreatorUserId == gallery.CreatorUserId &&
            u.Name == gallery.Name); if (data != null)
            {
                return true;
                //throw new UserFriendlyException(L("DuplicateFileName"));
            }
            return false;
        }

    }

}
