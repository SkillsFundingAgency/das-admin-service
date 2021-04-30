using Newtonsoft.Json;

namespace SFA.DAS.AdminService.Common.Settings
{
    public class RedisCacheSettings
    {
        [JsonRequired] public string RedisConnectionString { get; set; }
        [JsonRequired] public string DataProtectionKeysDatabase { get; set; }
        [JsonRequired] public string SessionCachingDatabase { get; set; }

    }
}
