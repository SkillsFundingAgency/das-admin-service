using SFA.DAS.Http.Configuration;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.QnA
{
    public class QnaApiClientConfiguration : IManagedIdentityClientConfiguration
    {
        public string IdentifierUri { get; set; }
        public string ApiBaseUrl { get; set; }
    }
}
