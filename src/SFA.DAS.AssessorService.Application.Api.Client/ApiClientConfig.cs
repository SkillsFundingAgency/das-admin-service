using Microsoft.Extensions.Hosting;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public class ApiClientConfig : IApiClientConfig
    {
        private readonly IWebConfiguration _webConfiguration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ApiClientConfig(IWebConfiguration webConfiguration, IHostingEnvironment hostingEnvironment)
        {
            _webConfiguration = webConfiguration;
            _hostingEnvironment = hostingEnvironment;
        }

        public string GetBaseAddress(ApplicationType applicationType)
        {
            if (applicationType == ApplicationType.RoATP)
            {
                return _webConfiguration.RoatpApplyApiAuthentication.ApiBaseAddress;
            }

            return _webConfiguration.ClientApiAuthentication.ApiBaseAddress;
        }

        public ITokenService GetTokenService(ApplicationType applicationType)
        {
            if (applicationType == ApplicationType.RoATP)
            {
                return new RoatpApplyTokenService(_webConfiguration, _hostingEnvironment);
            }

            return new TokenService(_webConfiguration, _hostingEnvironment);
        }
    }
}
