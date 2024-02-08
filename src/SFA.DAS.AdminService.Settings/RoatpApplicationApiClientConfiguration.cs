using SFA.DAS.Http.Configuration;

namespace SFA.DAS.AdminService.Settings
{
    public class RoatpApplicationApiClientConfiguration : IManagedIdentityClientConfiguration
    {
        public string ApiBaseUrl { get; set; }

        public string IdentifierUri { get; set; }
    }
}
