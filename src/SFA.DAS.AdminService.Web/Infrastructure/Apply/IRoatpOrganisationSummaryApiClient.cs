using System;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Infrastructure.Apply
{
    public interface IRoatpOrganisationSummaryApiClient
    {
        Task<string> GetTypeOfOrganisation(Guid applicationId);
    }
}
