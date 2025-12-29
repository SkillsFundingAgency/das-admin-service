using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.Roatp
{
    public interface IRoatpApiClient
    {
        Task<IEnumerable<IDictionary<string, object>>> GetCompleteRegister();
        Task<IEnumerable<IDictionary<string, object>>> GetAuditHistory();
    }
}
