using System.Collections.Generic;
using System.Threading.Tasks;
using Abp;
using CorarlERP.Dto;

namespace CorarlERP.Gdpr
{
    public interface IUserCollectedDataProvider
    {
        Task<List<FileDto>> GetFiles(UserIdentifier user);
    }
}
