using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp.Types;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp;

public interface IRoatpApiClient
{
    Task<GetAllOrganisationsResponse> GetCompleteRegister();
    Task<IEnumerable<IDictionary<string, object>>> GetAuditHistory();
}
