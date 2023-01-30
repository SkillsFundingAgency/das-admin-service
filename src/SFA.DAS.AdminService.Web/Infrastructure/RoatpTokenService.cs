using Microsoft.Extensions.Hosting;
using SFA.DAS.AdminService.Settings;
using Azure.Identity;
using Azure.Core;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class RoatpTokenService : IRoatpTokenService
    {
        private readonly IWebConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;
        public RoatpTokenService(IWebConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
        }
        public async Task<string> GetToken()
        {
            if (_hostEnvironment.IsDevelopment())
                return string.Empty;

            var tokenProvider = new DefaultAzureCredential();
            var tokenTask = await tokenProvider.GetTokenAsync(
                new TokenRequestContext(scopes: new string[] { _configuration.RoatpApiAuthentication.ResourceId + "/.default" }) { });

            return tokenTask.Token;
        }
    }
}
