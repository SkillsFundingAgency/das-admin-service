
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.AdminService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public class TokenService : ITokenService
    {
        private readonly IWebConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        public TokenService(IWebConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public string GetToken()
        {
            if (_hostingEnvironment.IsDevelopment())
                return string.Empty;

            var tenantId = _configuration.EpaoApiAuthentication.TenantId;// 
            var clientId = _configuration.EpaoApiAuthentication.ClientId;// 
            var appKey = _configuration.EpaoApiAuthentication.ClientSecret;// 
            var resourceId = _configuration.EpaoApiAuthentication.ResourceId;// 

            var authority = $"https://login.microsoftonline.com/{tenantId}";
            var clientCredential = new ClientCredential(clientId, appKey);
            var context = new AuthenticationContext(authority, true);
            var result = context.AcquireTokenAsync(resourceId, clientCredential).Result;
                
            return result.AccessToken;
        }
    }
}