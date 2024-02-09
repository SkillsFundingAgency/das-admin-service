using SFA.DAS.Http.Configuration;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp
{
    public class RoatpApiClientConfiguration : IManagedIdentityClientConfiguration
    {
        public string IdentifierUri { get; set; }
        public string ApiBaseUrl { get; set; }
    }
}
