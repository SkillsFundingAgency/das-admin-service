using SFA.DAS.Http;
using System.Net.Http;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication
{
    public class RoatpApplicationApiClientFactory : IRoatpApplicationApiClientFactory
    {
        private readonly RoatpApplicationApiClientConfiguration _roatpApplicationApiClientConfiguration;

        public RoatpApplicationApiClientFactory(RoatpApplicationApiClientConfiguration roatpApplicationApiClientConfiguration)
        {
            _roatpApplicationApiClientConfiguration = roatpApplicationApiClientConfiguration;
        }

        public HttpClient CreateHttpClient()
        {
            var httpClient = new ManagedIdentityHttpClientFactory(_roatpApplicationApiClientConfiguration).CreateHttpClient();
            return httpClient;
        }
    }
}
