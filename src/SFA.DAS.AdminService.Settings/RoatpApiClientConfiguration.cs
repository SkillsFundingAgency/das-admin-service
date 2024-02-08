using SFA.DAS.Http.Configuration;

namespace SFA.DAS.AdminService.Settings
{
    public class RoatpApiClientConfiguration : IManagedIdentityClientConfiguration
    {
        public string ApiBaseUrl { get; set; }

        public string IdentifierUri { get; set; }
    }
}
