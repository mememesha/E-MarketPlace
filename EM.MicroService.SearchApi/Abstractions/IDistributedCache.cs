using EM.MicroService.SearchApi.Models;

namespace EM.MicroService.SearchApi.Abstractions;

public interface IDistributedCache
{
    Task<bool> AddPreferencesToCache(Guid redisKey, byte[] preferenceResponses);
    Task<byte[]> GetPreferencesFromCache(Guid redisKey);
}
