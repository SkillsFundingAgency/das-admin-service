using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
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

            var tenantId = _configuration.ApplyApiAuthentication.TenantId;
            var clientId = _configuration.ApplyApiAuthentication.ClientId;
            var appKey = _configuration.ApplyApiAuthentication.ClientSecret;
            var resourceId = _configuration.ApplyApiAuthentication.ResourceId;

            var authority = $"https://login.microsoftonline.com/{tenantId}";
            var clientCredential = new ClientCredential(clientId, appKey);
            var context = new AuthenticationContext(authority, true);
            var result = context.AcquireTokenAsync(resourceId, clientCredential).Result;

            return result.AccessToken;
        }
    }
}