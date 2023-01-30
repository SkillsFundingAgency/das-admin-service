using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Hosting;
using SFA.DAS.AdminService.Settings;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class RoatpApplyTokenService : IRoatpApplyTokenService
    {
        private readonly IWebConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        public RoatpApplyTokenService(IWebConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public string GetToken()
        {
            if (_hostingEnvironment.IsDevelopment())
                return string.Empty;

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var generateTokenTask = azureServiceTokenProvider.GetAccessTokenAsync(_configuration.ApplyApiAuthentication.Identifier);

            return generateTokenTask.GetAwaiter().GetResult();
        }
    }
}
