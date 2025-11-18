using Abp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CorarlERP.RedisCaches
{
    public interface IRedisCacheManager : IDomainService
    {
        Task<T> GetDataAsync<T>(string key);
        Task<bool> SetDataAsync<T>(string key, T value);
        Task<bool> SetDataAsync<T>(string key, T value, DateTime expiryDate);
        Task<bool> RemoveDataAsync(string key);
        Task<bool> IsKeyExistAsync(string key);
        Task<bool> SetListDataAsync<T>(string key, T value);
        Task BulkSetListDataAsync<T>(string key, List<T> values);
    }
}
