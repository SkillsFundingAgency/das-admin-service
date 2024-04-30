using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AdminService.Common.Settings;
using StackExchange.Redis;

namespace SFA.DAS.AdminService.Web.Extensions
{
    public static class DataProtectionStartupExtensions
    {
        private const string ApplicationName = "das-admin-service-web";

        public static IServiceCollection AddDistributedCache(this IServiceCollection services, RedisCacheSettings redisCacheSettings, IHostingEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                services.AddDistributedMemoryCache();
                services.AddDataProtection()
                    .SetApplicationName(ApplicationName);
            }
            else
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = $"{redisCacheSettings.RedisConnectionString},{redisCacheSettings.SessionCachingDatabase}";
                });

                var redis = ConnectionMultiplexer.Connect($"{redisCacheSettings.RedisConnectionString},{redisCacheSettings.DataProtectionKeysDatabase}");
                services.AddDataProtection()
                    .SetApplicationName(ApplicationName)
                    .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
            }
            return services;
        }
    }
}
