using Microsoft.Extensions.Hosting;
using SFA.DAS.AdminService.Settings;
//using Azure.Identity;
//using Azure.Core;

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
        public string GetToken()
        {
            if (_hostEnvironment.IsDevelopment())
                return string.Empty;

            //var result = new DefaultAzureCredential().GetTokenAsync(
            //    new TokenRequestContext(scopes: new string[] { _configuration.RoatpApiAuthentication.ResourceId + "/.default" }) { }).Result;

            //return result.Token;
            return string.Empty;
        }
    }
}