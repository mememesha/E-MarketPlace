using System.Security.Cryptography;
using System.Text;

namespace EM.MicroService.SearchApi.Helpers;

public static class RedisKeyHelper
{
    public static Guid GetRedisKey(string query)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.Default.GetBytes(query));
        return new Guid(hash);
    }
}