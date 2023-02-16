using System.Text.Json.Serialization;

namespace SFA.DAS.AdminService.Common.Settings
{
    public class RedisCacheSettings
    {
        [JsonInclude] public string RedisConnectionString { get; set; }
        [JsonInclude] public string DataProtectionKeysDatabase { get; set; }
        [JsonInclude] public string SessionCachingDatabase { get; set; }

    }
}
