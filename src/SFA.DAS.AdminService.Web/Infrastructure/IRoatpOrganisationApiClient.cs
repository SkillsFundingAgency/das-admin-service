using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public interface IRoatpOrganisationApiClient
    {
        Task<Organisation> GetOrganisation(Guid id);
    }
}
