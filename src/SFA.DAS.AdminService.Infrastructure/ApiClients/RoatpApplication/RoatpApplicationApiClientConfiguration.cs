using SFA.DAS.Http.Configuration;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication
{
    public class RoatpApplicationApiClientConfiguration : IManagedIdentityClientConfiguration
    {
        public string ApiBaseUrl { get; set; }

        public string IdentifierUri { get; set; }
    }
}
