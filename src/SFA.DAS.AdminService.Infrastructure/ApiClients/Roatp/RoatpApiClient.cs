using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp
{
    public class RoatpApiClient : RoatpApiClientBase<RoatpApiClient>, IRoatpApiClient
    {
        public RoatpApiClient(IRoatpApiClientFactory clientFactory, ILogger<RoatpApiClient> logger)
            : base(clientFactory.CreateHttpClient(), logger)
        {
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetAuditHistory()
        {
            string url = $"/api/v1/download/audit";
            _logger.LogInformation("Retrieving RoATP register audit history data from {Url}", url);

            return await Get<List<IDictionary<string, object>>>(url);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetCompleteRegister()
        {
            string url = $"/api/v1/download/complete";
            _logger.LogInformation("Retrieving RoATP complete register data from {Url}", url);
            return await Get<List<IDictionary<string, object>>>(url);
        }
    }
}
