using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public interface IRoatpOrganisationApiClient
    {
        Task<Organisation> GetOrganisation(Guid id);
    }
}
