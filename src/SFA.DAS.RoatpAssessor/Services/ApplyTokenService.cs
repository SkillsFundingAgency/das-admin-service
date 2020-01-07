using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.AdminService.Settings;
using SFA.DAS.AssessorService.Application.Api.Client;

namespace SFA.DAS.RoatpAssessor.Services
{
    public class ApplyTokenService : IApplyTokenService
    {
        private readonly IWebConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        public ApplyTokenService(IWebConfiguration configuration, IHostingEnvironment hostingEnvironment)
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
