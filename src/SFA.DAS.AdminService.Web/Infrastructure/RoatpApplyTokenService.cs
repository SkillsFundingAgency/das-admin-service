using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Hosting;
using SFA.DAS.AdminService.Settings;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class RoatpApplyTokenService : IRoatpApplyTokenService
    {
        private readonly IWebConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;
        public RoatpApplyTokenService(IWebConfiguration configuration, IHostEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostEnvironment = hostingEnvironment;
        }

        public async Task<string> GetToken()
        {
            if (_hostEnvironment.IsDevelopment())
                return string.Empty;

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var generateTokenTask = await azureServiceTokenProvider.GetAccessTokenAsync(_configuration.ApplyApiAuthentication.Identifier);

            return generateTokenTask;
        }
    }
}