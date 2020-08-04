using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Extensions;
using System.Security.Claims;

namespace SFA.DAS.AdminService.Common
{
    public static class DependencyInjection
    {
        public static void ConfigureDependencyInjection(IServiceCollection services)
        {

            UserExtensions.Logger = services.BuildServiceProvider().GetService<ILogger<ClaimsPrincipal>>();
        }
    }
}
