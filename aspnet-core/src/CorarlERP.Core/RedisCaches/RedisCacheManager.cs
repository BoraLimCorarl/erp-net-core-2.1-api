
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Amazon.Util.Internal;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Linq;
using System.Text;
using CorarlERP.Configuration;
using Microsoft.Extensions.Configuration;

namespace CorarlERP.RedisCaches
{
    public class RedisCacheManager : CorarlERPDomainServiceBase,  IRedisCacheManager
    {
        private readonly IDistributedCache _cache;           
        public RedisCacheManager(IDistributedCache cache)
        {
          
               
            _cache = cache;
        }

        public async Task<T> GetDataAsync<T>(string key)
        {


            try
            {
                var val = _cache.Get(key);

                if (val == null) return default;
                var value = System.Text.Json.JsonSerializer.Deserialize<T>(val, GetJsonSerializerOptions());
                return value;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        private static System.Text.Json.JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new System.Text.Json.JsonSerializerOptions()
            {
                PropertyNamingPolicy = null,
                WriteIndented = true,            
                AllowTrailingCommas = true,

            };
        }

        public async Task<List<T>> GetListDataAsync<T>(string key)
        {
            try
            {
                var result = new List<T>();
                var val = _cache.Get(key);
                if (val != null)
                {
                    result = System.Text.Json.JsonSerializer.Deserialize<List<T>>(val, GetJsonSerializerOptions());
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> IsKeyExistAsync(string key)
        {

            try
            {
                var val = await _cache.GetAsync(key);
                return val != null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> RemoveDataAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        public async Task<bool> SetDataAsync<T>(string key, T value)
        {


            try
            {
                await SetValueAsync(key, value, new DistributedCacheEntryOptions());

                return true;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);

            }
        }

        public async Task<bool> SetListDataAsync<T>(string key, T value)
        {


            try
            {
                await _cache.SetStringAsync(key, JsonConvert.SerializeObject(value));
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task BulkSetListDataAsync<T>(string key, List<T> values)
        {



            try
            {
                await _cache.SetStringAsync(key, JsonConvert.SerializeObject(values));
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);

            }
        }

        public async Task<bool> SetDataAsync<T>(string key, T value, DateTime expiryDate)
        {

            try
            {
                await SetValueAsync(key, value, new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = expiryDate
                });
                return true;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task BulkSetListDataAsync<T>(string key, List<T> values, DateTime expiryDate)
        {


            try
            {
                var val = _cache.Get(key);

                if (val != null)
                {

                    var value = System.Text.Json.JsonSerializer.Deserialize<List<T>>(val, GetJsonSerializerOptions());

                    if (value != null && value.Count() > 0)
                    {
                        values.AddRange(value);
                    }
                }


                await SetValueAsync(key, values, new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = expiryDate
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        private Task SetValueAsync<T>(string key, T value, DistributedCacheEntryOptions options)
        {
            var bytes = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(value, GetJsonSerializerOptions()));
            return _cache.SetAsync(key, bytes, options);
        }

    }
}
