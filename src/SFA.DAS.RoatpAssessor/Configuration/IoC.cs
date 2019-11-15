using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.RoatpAssessor.Application;
using SFA.DAS.RoatpAssessor.Services;

namespace SFA.DAS.RoatpAssessor.Configuration
{
    public static class IoC
    {
        public static void ConfigureServices(ClientApiAuthentication applyApiAuthentication, IServiceCollection services)
        {
            services.AddMediatR(typeof(IoC).Assembly);

            services.AddTransient<IApplyTokenService, ApplyTokenService>();

            services.AddTransient<IApplyApiClient>(x => new ApplyApiClient(
              applyApiAuthentication.ApiBaseAddress,
              x.GetService<IApplyTokenService>(),
              x.GetService<ILogger<ApplyApiClient>>()));
        }
    }
}
