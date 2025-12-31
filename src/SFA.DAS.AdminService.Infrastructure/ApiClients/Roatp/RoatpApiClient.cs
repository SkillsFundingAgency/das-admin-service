using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp;

public class RoatpApiClient : RoatpApiClientBase<RoatpApiClient>, IRoatpApiClient
{
    public RoatpApiClient(IRoatpApiClientFactory clientFactory, ILogger<RoatpApiClient> logger)
        : base(clientFactory.CreateHttpClient(), logger)
    {
    }

    public async Task<GetAllOrganisationAuditRecordsResponse> GetAuditHistory()
    {
        string url = $"/organisations/audit-records";
        _logger.LogInformation("Retrieving RoATP register audit history data from {Url}", url);

        return await Get<GetAllOrganisationAuditRecordsResponse>(url);
    }

    public async Task<GetAllOrganisationsResponse> GetCompleteRegister()
    {
        string url = $"/organisations";
        _logger.LogInformation("Retrieving RoATP complete register data from {Url}", url);
        return await Get<GetAllOrganisationsResponse>(url);
    }
}
